using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using testMVC.Models;
using MyServer.Models;
using MyServer.Controllers.utils;

namespace testMVC.Controllers {

    public class HomeController : Controller {

// 上传文件        
        public IActionResult Upload() {
            return View();
        }        
// 登录
        public ActionResult Login() {
            string userName = Request.Form["username"];
            string password = Request.Form["password"];
            if (userName == "admin" && password == "123") {
                // Authentication.Login(HttpContext, userName); // 这个类没有连好，因为参考的项目感觉并不能运行得很好到自己可以狠好的理解的程度
            }
            return Redirect("/Home/Index");
        }
// 登出
        public ActionResult Logout() {
            // Authentication.Logout(HttpContext);
            return Redirect("/Home/Index");
        }
        
        // public IActionResult Login() {
        //     return View();
        // }        
        // public IActionResult Logout() {
        //     return View();
        // }        

// 这里实现了可以向服务器上传多个文件，但无法上传文件夹，会过滤到内㠌文件夹        
        [HttpPost]　　　　// 上传文件是 post 方式，这里加不加都可以
        public async Task<IActionResult> UploadFiles(List<IFormFile> files) {
            long size = files.Sum(f => f.Length);       // 统计所有文件的大小
            var filepath = Directory.GetCurrentDirectory() + "\\android";  // 存储文件的路径
            ViewBag.log = "日志内容为：";     // 记录日志内容
            foreach (var item in files) { // 上传选定的文件列表 {
                if (item.Length > 0) { // 文件大小 0 才上传 {
                    var thispath = filepath + "\\" + item.FileName;     // 当前上传文件应存放的位置
                    if (System.IO.File.Exists(thispath) == true) { // 如果文件已经存在,跳过此文件的上传 {
                        ViewBag.log += "\r\n文件已存在：" + thispath.ToString();
                        continue;
                    }
                    // 上传文件
                    using (var stream = new FileStream(thispath, FileMode.Create)) { // 创建特定名称的文件流 {
                        try {
                            await item.CopyToAsync(stream);     // 上传文件
                        }
                        catch (Exception ex) { // 上传异常处理 {
                            ViewBag.log += "\r\n" + ex.ToString();
                        }
                    }
                }
            }
            return View();
        }        

        [HttpPost]
        public IActionResult Index(LoginModel model) {
            if (ModelState.IsValid)
            {
                //检查用户信息
                // var user = _userAppService.CheckUser(model.UserName, model.Password); // <<<<<<<<<<<<<<<<<<<< 这里需要一个功能
                var user = new User("me");
                if (user != null)
                {
                    //记录Session
                    HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(user));
                    //跳转到系统首页
                    return RedirectToAction("Index", "Home");
                }
                // ModelState.AddModelError("", "用户名或密码错误。");
                ViewBag.ErrorInfo = "用户名或密码错误。";
                return View();
            }
            ViewBag.ErrorInfo = ModelState.Values.First().Errors[0].ErrorMessage;
            return View(model);
        }
        // public IActionResult Index() {
        //     return View();
        // }
        public IActionResult Privacy() {
            return View();
        }

        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}