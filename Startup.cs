using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyServer {

    public class Startup {
        // public Startup() { }
        
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage(); // ErrorViewModel 用的地方 ?
            }
            app.UseStaticFiles(); // 
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers(); // 下面再添加一点儿:

                endpoints.MapGet("/", async context => {
                        await context.Response.WriteAsync("Hello World!");
                    });
                endpoints.MapGet("/Home/Login", async context => {
                        await context.Response.WriteAsync("Login");
                    });
                endpoints.MapGet("/Manage", async context => { // 这里可能会报错
                        await context.Response.WriteAsync("Manage");
                    }).RequireAuthorization();
                endpoints.MapRazorPages();
            });
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o => {
                    // 登录路径：这是当用户试图访问资源但未经过身份验证时，程序将会将请求重定向到这个相对路径
                    // o.LoginPath = new PathString("/Account/Login");
                    o.LoginPath = new PathString("/Home/Login");
                    // 禁止访问路径：当用户试图访问资源时，但未通过该资源的任何授权策略，请求将被重定向到这个相对路径。
                    o.AccessDeniedPath = new PathString("/Home/About");
                });

// 再添加一些别的:
            services.AddRazorPages();
            
            
            //services.AddDbContext<MyDbContext>(options => // .NET Core 5.0
            //                                   options.UseMySql(ConfigurationString("MySQL"),
            //                                                    MyServerVersion.LatestSupportedServerVersion));
            //services.AddControllers();
            
            //     // 添加身后的数据库: 这个参考的方法太古老，有太多编译错误，先放一下
            //     services.AddDbContext<ApplicationDbContext>(options =>
            //                                                 options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //     services.AddIdentity<ApplicaitonUser, IdentityRole>()
            //         .AddEntityFrameworkStores<ApplicaitonDbContext>()
            //         .AddDefaultTokenProvider(); // 那么就是说，这里使用了另一种的验证方式
            //     // Password Strength Setting
            //     services.Configure<IdentityOptions>(options => {
            //         // Password settings
            //         options.Password.RequireDigit = true;
            //         options.Password.RequiredLength = 4; // 8 Hhj1
            //         options.Password.RequireNonAlphanumeric = false;
            //         options.Password.RequireUppercase = true;
            //         options.Password.RequireLowercase = false;
            //         options.Password.RequiredUniqueChars = 4; // 6
            //         // Lockout settings
            //         options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            //         options.Lockout.MaxFailedAccessAttempts = 5; // 尝试失败5次后禁登录
            //         options.Lockout.AllowedForNewUsers = true;
            //         // User settings
            //         options.User.RequireUniqueEmail = true;
            //     });
            //     //Setting the Account Login page
            //     services.ConfigureApplicationCookie(options => {
            //         // Cookie settings
            //         options.Cookie.HttpOnly = true;
            //         options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            //         options.LoginPath = "/Home/Login"; // If the LoginPath is not set here,
            //         // ASP.NET Core will default to /Home/Login
            //         options.LogoutPath = "/Home/Logout"; // If the LogoutPath is not set here,
            //         // ASP.NET Core will default to /Home/Logout
            //         // options.AccessDeniedPath = "/Home/AccessDenied"; // If the AccessDeniedPath is 因为自己没有配置，暂时去掉
            //         // // not set here, ASP.NET Core
            //         // // will default to
            //         // // /Account/AccessDenied
            //         options.SlidingExpiration = true;
            //     });
            //     // 不知道下面的有什么相关
            //     services.AddTransient<IEmailSender, EmailSender>();
            //     services.AddMvc();
        }
    }
}
