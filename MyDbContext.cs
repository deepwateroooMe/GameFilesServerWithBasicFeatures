using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MvcDemo.Models;
using MyServer.Models;
using System;
using System.IO;

namespace MyServer {

    public class MyDbContext : DbContext {
        public DbSet<Account> Accounts { get; set; }

		public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) {
            this.Database.Migrate();
        }

		public MyDbContext()
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
// 这里跟连接的是：  哪个哪种数据库有关            
            // optionsBuilder.UseMySql(connectionString, b => b.MigrationsAssembly("AspNetCoreMultipleProject")); // 这里还是有点儿问题的: 字符串为空
        // "DefaultConnection": "Server=127.0.0.1;Database=test;uid=root;pwd=hhj1;port=80;charset=utf8mb4;"
            optionsBuilder.UseMySql("Server=127.0.0.1;Database=test;uid=root;pwd=hhj1;port=80;charset=utf8mb4;",
                                    new MySqlServerVersion(new Version(8, 0, 11)));
                                    // b=> b.MigrationsAssembly("AspNetCoreMultipleProject")); // 这里还是有点儿问题的: 会出现编译错误
// // 其它数据库参考：这里的问题是，某个库我没有安装好或是没有连接好，找不到方法
//             optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True");            
        }
    }
}
