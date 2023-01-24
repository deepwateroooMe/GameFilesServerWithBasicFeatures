using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer {
    public class Program {

        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();

// MySQL 数据库的初始化: 放在什么时机初始化,会比较好呢?
            MySqlConnection conn = new MySqlConnection("Server=localhost;database=test;uid=root;pwd=hhj1;port=80;charset=utf8mb4;"); // 数据库的连接字符串,还并没有连接,什么时候连接?
// 在这个数据库里,我先设计一个表: Users游戏用户,当然想要实现用户登录时的自动同步到数据库 isLogin? active, inactive (logout) 直接连接数据库,作好准备随时改写(中途会掉线吗?)
            conn.Open(); // 什么时候关闭释放资源
            if (conn.State == ConnectionState.Open) { // 打印相关的消息,这里可以跳过
                //MessageBox.Show("Connection Opened Successfully"); // 就当是自己验证一下,给自己一个可看的窗口,移植过来试试看: 应用类型不一样,这个MVC可以添加窗体吗? 不能简单地直接移植,算了
                //print_in_dataGridView();
            }
// 连接数据库狠好连,GRUD操作也狠简单,但是其它登录服相关的验证等不是狠懂,再回去读一遍ET框架中的相关功能模块源码
            
            // // 下面这个是连SQL Server的字符串
            // SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""F:\Microsoft Visual Studio\Project\Bookshop\DataBase\Regedit.mdf"";Integrated Security=True;Connect Timeout=30"); //新建一个全局数据库对象，使用该对象来操作全局的,@""是连接字符串，用于获取/打开SQLserver

// // 2019：可以连MySQL本地计算机上的数据库,但是不方便,不想从2022切换到2019,3但迫于时间,这里暂时还是先连自己电脑上的MySQL数据库服务器
            
// // 创建用于查询 Azure SQL 数据库中的数据库: 感觉这个方法不好,不想用Azure SQL,直接连SQL Server不行吗?
//             try {
//                 SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
//                 builder.DataSource = "<your_server>.database.windows.net"; 
//                 builder.UserID = "<your_username>";            
//                 builder.Password = "<your_password>";     
//                 builder.InitialCatalog = "<your_database>";

//                 using (SqlConnection connection = new SqlConnection(builder.ConnectionString)) {
//                     Console.WriteLine("\nQuery data example:");
//                     Console.WriteLine("=========================================\n");

//                     String sql = "SELECT name, collation_name FROM sys.databases";

//                     using (SqlCommand command = new SqlCommand(sql, connection)) {
//                         connection.Open();
//                         using (SqlDataReader reader = command.ExecuteReader()) {
//                             while (reader.Read()) {
//                                 Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
//                             }
//                         }
//                     }                    
//                 }
//             }
//             catch (SqlException e) {
//                 Console.WriteLine(e.ToString());
//             }
//             Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseStartup<Startup>();
            }).ConfigureAppConfiguration((context, config) => {
                var env = context.HostingEnvironment;
                config.AddJsonFile("redis.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"redis.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args);
            });


//         static void	Main(string[] args) {
//             var builder = WebApplication.CreateBuilder(args); // .NET 7.0
            
// // Add services to the container.
//             builder.Services.AddControllersWithViews();

// // // 添加数据库: 这几个例子都仍然还是缺少了点儿数据库的连接，配置乖相关的步骤
// // //builder.Services.AddDbContext<MvcMovieContext>(options =>
// // //                                               options.UseSqlServer(builder.Configuration.GetConnectionString("MvcMovieContext")));
// // builder.Services.AddDbContext<Girl1804Context>(options => {
// //         options.UseSqlServer(Configuration.GetConnectionString("Girl1804DB"));
// //     });
// // // 引入跨域服务
// // builder.Services.AddCors(options =>
// //                          options.AddPolicy(MyCorsPolicy, builder1 => {
// //                              builder1.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
// //                              builder.Services.AddControllers();
// //                          }));
// // builder.Services.AddControllers();
// // builder.Services.AddSwaggerGen(c => {
// //     c.SwaggerDoc("v1", new OpenApiInfo{
// //             Title = "API", Version = "v1"});
// //     var basePath = Path.GetDirectionName(typeof(Program).Assembly.Location);
// //     var xmlPath = Path.Combine(basePath, "API.xml");
// //     c.IncludeXmlComments(xmlPath);
// // });
// // // 添加EF模型　
// // builder.Services.AddDbContext<zjkcsContext>(options =>
// //                                             options.UseSqlServer(Configuration.GetConnectionString("zjkcsDB")));

//             //var connectionString = builder.Configuration.GetConnectionString("ConnectionStrings");
//             //builder.Services.AddDbContext<MyDbContext>(x => x.UseSqlServer(connectionString));

//             var app = builder.Build(); 

// // Configure the HTTP request pipeline.
//             if (!app.Environment.IsDevelopment()) {
//                 app.UseExceptionHandler("/Home/Error"); // 这里会用到的
//                 // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//                 app.UseHsts();
//             }
//             app.UseHttpsRedirection();
//             app.UseStaticFiles();

//             app.UseRouting();

//             app.UseAuthorization(); // 进行身份验证

//             app.MapControllerRoute(
//                 name: "default",
//                 pattern: "{controller=Home}/{action=Index}/{id?}");
//             app.Run();
//         }
    }
}			