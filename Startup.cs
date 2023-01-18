using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyServer {
    public class Startup {
        
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services) {
// // 把下面自己原本写得不全的，理解得半生不熟的，再改写一遍
//             services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//                 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o => {
//                     // 登录路径：这是当用户试图访问资源但未经过身份验证时，程序将会将请求重定向到这个相对路径
//                     // o.LoginPath = new PathString("/Account/Login");
//                     o.LoginPath = new PathString("/Home/Login");
//                     // 禁止访问路径：当用户试图访问资源时，但未通过该资源的任何授权策略，请求将被重定向到这个相对路径。
//                     o.AccessDeniedPath = new PathString("/Home/About");
//                 }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) 
//                 .AddCookie(IdentityConstants.ApplicationScheme);
//             services.AddRazorPages();  // 再添加一些别的:

            // 将选项配置提出来是为了在配置时使用 DI服务 
            services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<IDataProtectionProvider, IDistributedCache>((options, dp, distributedCache) => {
                    // set-cookie: https:// developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie
                    // 默认配置参考ASP.NET Core源码：class CookieAuthenticationOptions
                    // 默认后期配置参考ASP.NET Core源码：class PostConfigureCookieAuthenticationOptions
                    // 以下是当前 Cookie 认证方案的全局配置，部分项可以在实际使用时重写
                    // 以下4个配置均在 CookieAuthenticationDefaults 类中有对应的默认值
                    // 登录路径
                    options.LoginPath = new PathString("/Account/Login");
                    // 注销路径
                    options.LogoutPath = new PathString("/Account/Logout");
                    // 禁止访问路径
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                    // returnUrl参数名，默认 ReturnUrl
                    options.ReturnUrlParameter = "returnUrl";
                    // Cookie 中 authentication ticket 的有效期（注：不是Cookie的有效期。当然，如果 Cookie 都没了，它也就失效了）
                    // 票据是否过期是通过该Expires与当前时间作比较来判断的
                    // 当声明 AuthenticationProperties.Persistent = true 时：
                    //      通过(AuthenticationProperties.IssuedUtc + ExpireTimeSpan)得到一个具体的时间点，会作为 Cookie 的 Expires 属性，
                    //      此时，如果没有设置 MaxAge，则该值也是Cookie的有效期
                    // 默认 14 天
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                    // Cooke 的属性 Expires，指示 Cookie 在浏览器中的保存时间
                    // 目前该字段已被禁用，应该使用 ExpireTimeSpan，只有当票据持久化时，才设置到 Expires 属性上
                    // options.Cookie.Expiration = TimeSpan.FromMinutes(30);
                    // Cookie 在浏览器中的保存时间
                    // 如果 MaxAge 和 Expires 同时设置，则以 MaxAge 为准
                    // 如果以上两者均未设置，则该 Cookie 生命周期为 session cookie，即浏览器关闭后，Cookie会被清空（部分浏览器有会话恢复功能）
                    // 一般无需显式设置该字段
                    // options.Cookie.MaxAge = TimeSpan.FromDays(14);
                    // Cookie 的过期方式是否为滑动过期
                    // 设置为 true 时，服务端收到请求，若发现 Cookie 的生存期已经超过了一半，服务端会重新颁发 Cookie（注：authentication ticket 也是新的）
                    // 默认 true
                    options.SlidingExpiration = true;
                    // Cookie 的名字，默认是 .AspNetCore.Cookies
                    options.Cookie.Name = "auth";
                    // 有权限访问 Cookie 的域
                    // options.Cookie.Domain = ".xxx.cn";
                    // 有权限访问 Cookie 的路径
                    options.Cookie.Path = "/";
                    // 允许 跨站的第三方站点 向 当前站点 发送GET请求时 携带Cookie （防止CSRF攻击）
                    // 默认 SameSiteMode.Lax
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                    // 禁止客户端脚本访问Cookie（防止XSS攻击）
                    // 默认 true
                    options.Cookie.HttpOnly = true;
                    // 只有当颁发 Cookie 的 URI 是 Https 时，才设置 Secure = true
                    // 默认 CookieSecurePolicy.SameAsRequest
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    // 指示该 Cookie 对于应用的正常运行是必要的，不需要经过用户同意使用
                    // 默认 true
                    options.Cookie.IsEssential = true;
                    // 设置 Cookie 管理器
                    // 默认 ChunkingCookieManager
                    options.CookieManager = new ChunkingCookieManager();
                    // 用于加密和解密 Cookie 中的 authentication ticket
                    // 以下为默认值
                    options.DataProtectionProvider ??= dp;
                    var dataProtector = options.DataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", CookieAuthenticationDefaults.AuthenticationScheme, "v2");
                    options.TicketDataFormat = new TicketDataFormat(dataProtector);
                    // 登录前回调
                    options.Events.OnSigningIn = context => {
                        Console.WriteLine($"{context.Principal.Identity.Name} 正在登录...");
                        return Task.CompletedTask;
                    };
                    // 登录后回调
                    options.Events.OnSignedIn = context => {
                        Console.WriteLine($"{context.Principal.Identity.Name} 已登录");
                        return Task.CompletedTask;
                    };
                    // 注销时回调
                    options.Events.OnSigningOut = context => {
                        Console.WriteLine($"{context.HttpContext.User.Identity.Name} 注销");
                        return Task.CompletedTask;
                    };
                    // 验证 Principal 时回调
                    options.Events.OnValidatePrincipal = async context => {
                        Console.WriteLine($"{context.Principal.Identity.Name} 验证 Principal");
                        var userId = context.Principal.Identities.First().Claims.First(c => c.Type == JwtClaimTypes.Id).Value;
                        var cacheKey = $"delete-auth-cookie:{userId}";
                        if (!string.IsNullOrWhiteSpace(await distributedCache.GetStringAsync(cacheKey))) {
                            context.RejectPrincipal();
                            await distributedCache.RemoveAsync(cacheKey);
                        }
                    };
                    options.Events.OnRedirectToLogin = async context => {
                        if (IsAjaxRequest(context.Request)) {
                            context.Response.Headers[HeaderNames.Location] = context.RedirectUri;
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsJsonAsync(new { Message = "登录已过期，请重新登录" });
                        }
                        else {
                            // 添加额外参数
                            // context.RedirectUri += $"&now={DateTime.Now:o}";
                            context.Response.Redirect(context.RedirectUri);
                        }
                    };
                    // （取消注释）将会话信息存储在服务端内存
                    // options.SessionStore = new MemoryCacheTicketStore(options.ExpireTimeSpan);
                    // （取消注释）将会话信息存储在服务端分布式缓存
                    // options.SessionStore = new DistributedCacheTicketStore(distributedCache, options.ExpireTimeSpan);
                });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
            // Cookie全局策略
            services.AddCookiePolicy(options => {
                options.OnAppendCookie = context => {
                    Console.WriteLine("------------------ On Append Cookie --------------------");
                    Console.WriteLine($"Name: {context.CookieName}\tValue: {context.CookieValue}");
                };
                options.OnDeleteCookie = context => {
                    Console.WriteLine("------------------ On Delete Cookie --------------------");
                    Console.WriteLine($"Name: {context.CookieName}");
                };
            });
            services.AddAuthorization(options => {
                // 授权配置，以下均使用的默认值
                // 默认的授权策略
                // 默认为要求通过身份认证的用户
                options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                // 当存在多个授权处理器时，若其中一个失败后，后续的处理器是否还继续执行
                // 默认 true
                options.InvokeHandlersAfterFailure = true;
            });
            services.AddControllersWithViews();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage(); // ErrorViewModel 用的地方 
            } else {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles(); // 
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
// // 把这里下面的全部都重新改一遍                
//                 endpoints.MapControllers(); // 下面再添加一点儿:
//                 endpoints.MapGet("/", async context => {
//                         await context.Response.WriteAsync("Hello World!");
//                     });
//                 endpoints.MapGet("/Home/Login", async context => {
//                         await context.Response.WriteAsync("Login");
//                     });
//                 // endpoints.MapGet("/Home/Upload", async context => {
//                 //     await context.Response.WriteAsync("Upload");
//                 // });
//                 endpoints.MapGet("/Manage", async context => { // 这里可能会报错
//                         await context.Response.WriteAsync("Manage");
//                     }).RequireAuthorization();
//                 endpoints.MapRazorPages();
            });
        }
// 把这一点儿的原理弄懂
        private static bool IsAjaxRequest(HttpRequest request) { 
            return string.Equals(request.Query[HeaderNames.XRequestedWith], "XMLHttpRequest", StringComparison.Ordinal) ||
                string.Equals(request.Headers[HeaderNames.XRequestedWith], "XMLHttpRequest", StringComparison.Ordinal);
        }
    }
}
