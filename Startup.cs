using Microsoft.AspNetCore.Authentication.Cookies;

namespace testMVC {

    public class Startup {

        public void ConfigureServices(IServiceCollection services) {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o => {
// 登录路径：这是当用户试图访问资源但未经过身份验证时，程序将会将请求重定向到这个相对路径
                    // o.LoginPath = new PathString("/Account/Login");
                    o.LoginPath = new PathString("/Home/Login");
// 禁止访问路径：当用户试图访问资源时，但未通过该资源的任何授权策略，请求将被重定向到这个相对路径。
                    o.AccessDeniedPath = new PathString("/Home/Privacy");
                });
        }
    }
}
