namespace MyServer {
    public class Program {
		static void	Main(string[] args) {
        
            var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
            builder.Services.AddControllersWithViews();

// // 添加数据库: 这几个例子都仍然还是缺少了点儿数据库的连接，配置乖相关的步骤
// //builder.Services.AddDbContext<MvcMovieContext>(options =>
// //                                               options.UseSqlServer(builder.Configuration.GetConnectionString("MvcMovieContext")));
// builder.Services.AddDbContext<Girl1804Context>(options => {
//         options.UseSqlServer(Configuration.GetConnectionString("Girl1804DB"));
//     });
// // 引入跨域服务
// builder.Services.AddCors(options =>
//                          options.AddPolicy(MyCorsPolicy, builder1 => {
//                              builder1.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
//                              builder.Services.AddControllers();
//                          }));
// builder.Services.AddControllers();
// builder.Services.AddSwaggerGen(c => {
//     c.SwaggerDoc("v1", new OpenApiInfo{
//             Title = "API", Version = "v1"});
//     var basePath = Path.GetDirectionName(typeof(Program).Assembly.Location);
//     var xmlPath = Path.Combine(basePath, "API.xml");
//     c.IncludeXmlComments(xmlPath);
// });
// // 添加EF模型　
// builder.Services.AddDbContext<zjkcsContext>(options =>
//                                             options.UseSqlServer(Configuration.GetConnectionString("zjkcsDB")));

            var connectionString = builder.Configuration.GetConnectionString("ConnectionStrings");
            builder.Services.AddDbContext<MyDbContext>(x => x.UseSqlServer(connectionString));

            var app = builder.Build(); 

// Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization(); // 进行身份验证

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}			
