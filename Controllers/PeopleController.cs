using Microsoft.AspNetCore.Mvc;
using MyServer.Models;

namespace MyServer.Controllers {

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PeopleController : ControllerBase {

        private readonly MyDbContext _dbContext;
        public PeopleController(MyDbContext dbContext) {
            _dbContext = dbContext;
        }

        // 创建
        [HttpGet]
        public IActionResult Create() {
            var message = "";
            using (_dbContext) {
                var person = new Person {
                    FirstName = "Rector",
                    LastName = "Liu",
                    CreatedAt = DateTime.Now
                };
                _dbContext.People.Add(person);
                var i = _dbContext.SaveChanges();
                message = i > 0 ? "数据写入成功" : "数据写入失败";
            }
            return Ok(message);
        }

// 读取指定Id的数据
        [HttpGet]
        public IActionResult GetById(int id) {
            using (_dbContext) {
                var list = _dbContext.People.Find(id);
                return Ok(list);
            }
        }

        // 读取所有
        [HttpGet]
        public IActionResult GetAll() {
            using (_dbContext) {
                var list = _dbContext.People.ToList();
                return Ok(list);
            }
        }
    }
}