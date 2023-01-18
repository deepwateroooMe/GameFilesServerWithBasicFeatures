using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using MyServer.Models;
using Microsoft.AspNetCore.Authorization;

namespace MyServer.Controllers {

	public class HomeController : Controller {
        private const string TAG = "HomeController";

        public IActionResult Index() {
            return View();
        }

        public IActionResult About() {
            return View();
        }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

// 视图中会调用这个方法
        [HttpGet]
        [Authorize]
        public JsonResult GetMessage() {
            return Json(new { Message = $"Hello! {User.Identity.Name}" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
           return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

// // 上传文件        
//         public IActionResult Upload() {
//             return View();
//         }        


// // 这里实现了可以向服务器上传多个文件，但无法上传文件夹，会过滤到内㠌文件夹        
//         [HttpPost, DisableRequestSizeLimit]　　　　// 上传文件是 post 方式，这里加不加都可以
//         public async Task<IActionResult> UploadFiles(List<IFormFile> files) {
//             long size = files.Sum(f => f.Length);       // 统计所有文件的大小

//             // 服务器将要存储文件的路径
//             // var Folder = AppDomain.CurrentDomain.BaseDirectory + "AppFileUploads/";
//             var filepath = AppDomain.CurrentDomain.BaseDirectory + "android/";
//             // Console.WriteLine(TAG + " filepath = " + filepath);

//             // var filepath = Directory.GetCurrentDirectory() + "\\android";  // 存储文件的路径: 这个路径极其误导人,其它应该是设置为服务器的存储文件路径
//             // if (!Directory.Exists(filepath)) { // 如果不存在就创建file文件夹: 感觉这里创建的地址不对!!!
//             //     Directory.CreateDirectory(filepath);
//             // }
//             ViewBag.log = "日志内容为：";     // 记录日志内容
//             foreach (var item in files) { // 上传选定的文件列表 
//                 if (item.Length > 0) { // 文件大小 0 才上传 
//                     var thispath = filepath + "\\" + item.FileName;     // 当前上传文件应存放的位置
// //// 对上传文件的后缀进行过滤:若不是资源包文件,跳过                    
// //                    var suf = Path.GetExtension(item.Files[0]); // 不知道这里是怎么转的了
// //                    if (suf != ".ab") {
// //                        // 这里是想直接返回错误的
// //                        ViewBag.log += "\r\n" + "不支持该文件格式，请上传 .ab后缀的资源包文件";
// //                        // return ErrorResult("不支持该文件格式，请上传 .ab后缀的资源包文件", 111113);
// //                    }
//                     // if (System.IO.File.Exists(thispath)) { // 如果文件夹不存在,则创建一个
//                     //     ViewBag.log += "\r\n文件已存在：" + thispath.ToString();
//                     //     continue;
//                     // }
//                     // 上传文件: 若文件存在,先删除文件,再创建新的.为什么要这样呢?
//                     if (System.IO.File.Exists(thispath)) {
//                         System.IO.File.Delete(thispath);
//                     }
//                     using (var stream = new FileStream(thispath, FileMode.Create)) { // 创建特定名称的文件流 
//                         try {
//                             await item.CopyToAsync(stream); // 上传文件: 这里是异步的
//                         }
//                         catch (Exception ex) { // 上传异常处理 {
//                             ViewBag.log += "\r\n" + ex.ToString();
//                         }
//                     }
//                     // using (FileStream fs = System.IO.File.Create(thispath)) { // 不知道这里写得对不对
//                     //     // 复制文件
//                     //     item.CopyTo(fs);
//                     //     // 清空缓冲区数据
//                     //     fs.Flush();
//                     // }
//                 }
//             }
//             return Json("Upload Successful.");
//             // return View();
//         }        

// // // 登录页面
// //         public IActionResult Login() {
// //             return View();
// //         }

// // // post 登录请求
// //         [HttpPost]
// //         public async Task<IActionResult> Login(string userName, string password) {
// //             if (userName.Equals("admin") && password.Equals("123")) {
// //                 var claims = new List<Claim>(){
// //                     new Claim(ClaimTypes.Name, userName), new Claim("password", password)
// //                 };
// //                 var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Customer"));
// //                 await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties {
// //                         ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
// //                         IsPersistent = false,
// //                         AllowRefresh = false
// //                     });
// //                 return Redirect("/Home/Index");
// //             }
// //             return Json(new { result = false, msg = "用户名密码错误!" });
// //         }
// // 退出登录
//         public async Task<IActionResult> Logout() {
//             await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//             return Redirect("/Login");
//         }
    }
}