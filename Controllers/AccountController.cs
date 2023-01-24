using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MyServer.Models;
using MyServer.Models.Account;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyServer.Controllers {

    public class AccountController : Controller {
        private readonly IOptionsMonitor<CookieAuthenticationOptions> _cookieAuthOptionsMonitor;
        private readonly IDistributedCache _cache;

// 当与数据库连起来的时候，应该是需要有数据库的上下文的
        private MyDbContext db = new MyDbContext();
        
        public AccountController(
            IOptionsMonitor<CookieAuthenticationOptions> cookieAuthOptions, IDistributedCache cache) {
            _cookieAuthOptionsMonitor = cookieAuthOptions;
            _cache = cache;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login([FromQuery] string returnUrl = "/") { // wired way of fixing it
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginViewModel input) {
            ViewBag.ReturnUrl = input.ReturnUrl; // 这里设置的值: 但是总是报错说,这里的没有设置或是弄错了?
                                                 // 用户名密码相同视为登录成功
            if (input.UserName != input.Password) {
                ModelState.AddModelError("UserNameOrPasswordError", "无效的用户名或密码");
            }
            if (!ModelState.IsValid) { // 状态不对
                return View(); // <<<<<<<<<< 总会走到这里返回
            }
            // 参数 authenticationType 必须非空或空白字符串，这样 identity.IsAuthenticated 才会是 true
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, JwtClaimTypes.Name, JwtClaimTypes.Role);
            identity.AddClaims(new[] {
                    // new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")),
                    // new Claim(ClaimTypes.Name, input.UserName)
                    new Claim(JwtClaimTypes.Id, Guid.NewGuid().ToString("N")),
                    new Claim(JwtClaimTypes.Name, input.UserName)
                });
            var principal = new ClaimsPrincipal(identity);
            // 登录
            // 内部会自动对 cookie 进行加密
            var properties = new AuthenticationProperties {
                // 票据所在的Cookie是否持久化。默认非持久化，即该Cookie有效期是会话级别
                // 若为 true，则会将 ExpiresUtc 的值设置到 Cookie 的 Expires 属性上
                IsPersistent = input.RememberMe,
                // Cookie 中 authentication ticket 的过期时间
                // 重写 CookieAuthenticationOptions.ExpireTimeSpan 的值，如果不设置该值，则取 CookieAuthenticationOptions.ExpireTimeSpan
                // ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(60),
                // 当Cookie为滑动过期时，允许重新颁发Cookie
                // 默认null，等同于 true
                AllowRefresh = true,
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
#region 以下简要模拟 SignInAsync 内部细节，更多细节请查看 AuthenticationService 和 CookieAuthenticationHandler
            // var options = _cookieAuthOptionsMonitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);
            // var ticket = new AuthenticationTicket(principal, properties, CookieAuthenticationDefaults.AuthenticationScheme);
            // ticket加密
            // var cookieValue = options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding(HttpContext));
            // options.CookieManager.AppendResponseCookie(HttpContext, options.Cookie.Name, cookieValue, new CookieOptions());
#endregion
#region 添加自定义Cookie
            Response.Cookies.Append("author", "xiaoxiaotank", new CookieOptions {
                    MaxAge = TimeSpan.FromSeconds(30)
                });
#endregion
            if (Url.IsLocalUrl(input.ReturnUrl)) {
                return Redirect(input.ReturnUrl);
            }
            return Redirect("/");
        }
        
        private static string GetTlsTokenBinding(HttpContext context) {
            var binding = context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        // 模拟更新账户信息后，用户需要重新登录
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateAccount() {
            // ... 更新信息
            var userId = User.Identities.First().Claims.First(c => c.Type == JwtClaimTypes.Id).Value;
            await _cache.SetStringAsync($"delete-auth-cookie:{userId}", userId);
            return Ok();
        }

        [HttpGet]
        public IActionResult AccessDenied([FromQuery] string returnUrl = null) {
            return View();
        }
        // // 登录页面
        //         public IActionResult Login() {
        //             return View();
        //         }
        // // post 登录请求
        //         [HttpPost]
        //         public async Task<IActionResult> Login(string userName, string password) {
        //             if (userName.Equals("admin") && password.Equals("123")) {
        //                 var claims = new List<Claim>(){
        //                     new Claim(ClaimTypes.Name, userName), new Claim("password", password)
        //                 };
        //                 var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Customer"));
        //                 await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties {
        //                         ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
        //                         IsPersistent = false,
        //                         AllowRefresh = false
        //                     });
        //                 return Redirect("/Home/Index");
        //             }
        //             return Json(new { result = false, msg = "用户名密码错误!" });
        //         }
        // // 退出登录 : 暂时，等解决一个bug
        //         public async Task<IActionResult> Logout() {
        //             await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //             return Redirect("/Login");
        //         }
    }
}