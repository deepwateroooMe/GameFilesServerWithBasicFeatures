using MyServer.Models;
using System.Data.Entity;

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
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
