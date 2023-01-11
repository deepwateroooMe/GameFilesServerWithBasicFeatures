using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyServer.Models;
using System;
using System.IO;

namespace MyServer {

    public class MyDbContext : DbContext {

        public DbSet<Person> People {
            get;
            set;
        }

		//public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) {
  //      }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            // optionsBuilder.UseMySql(connectionString, b => b.MigrationsAssembly("AspNetCoreMultipleProject")); // 这里还是有点儿问题的
            string tmp = "";
            optionsBuilder.UseMySql("Server=127.0.0.1;Database=game;Trusted_Connection=True;MultipleActiveResultSets=true",
                                    new MySqlServerVersion(new Version(8, 0, 11)));
                                    // b=> b.MigrationsAssembly("AspNetCoreMultipleProject")); // 这里还是有点儿问题的: 会出现编译错误
        }
    }
}