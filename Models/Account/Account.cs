using MyServer;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MvcDemo.Models {

    public class Account {
        private string AdminName = "admin";
        private string AdminPassword = "admin";

        private MyDbContext db = new MyDbContext();

        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("用户名")]
        public string UserName { get; set; }

        [Required]
        [DisplayName("用户密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // 注册验证密码，不用保存
        [NotMapped]
        [DisplayName("密码确认")]
        [DataType(DataType.Password)]
        public string ConfirmedPassword { get; set; }

        // 该方法应从数据库中对比用户名和密码
        public bool IsValid() {
            foreach (var item in db.Accounts) {
                if (item.UserName == this.UserName && item.Password == this.Password) {
                    return true;
                }
            }
            // 默认管理员密码
            if (this.UserName == AdminName & this.Password == AdminPassword) {
                return true;
            }
            return false;
        }

        // 是否是管理员登录
        public bool IsAdministrator() {
            // 默认管理员密码
            if (this.UserName == AdminName & this.Password == AdminPassword) {
                return true;
            }
            return false;
        }

        // 判断是否已经存在相同用户名
        public bool IsExtends() {
            foreach (var item in db.Accounts) {
                if (item.UserName == this.UserName) {
                    return true;
                }
            }
            return false;
        }

        // 注册密码验证
        public bool IsPasswordConfirmed() {
            if (this.Password == this.ConfirmedPassword) {
                return true;
            }
            return false;
        }
    }
}
