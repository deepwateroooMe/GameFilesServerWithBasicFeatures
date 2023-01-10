using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyServer.Models;
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

            var connectionString = configuration.GetConnectionString("AppDb");
            optionsBuilder.UseSqlServer(connectionString); // 这里有个找不到的方法或是动态生成的文件
        }
    }
}