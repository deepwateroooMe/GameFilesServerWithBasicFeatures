using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MyServer.Models.Account {

    public class LoginViewModel { 


        
        [Required(ErrorMessage = "用户名不能为空。")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密码不能为空。")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "记住我")]
        public bool RememberMe { get; set; }

// 当需要 用户注册 的页面,就需要用户邮箱,方便必要时候的与用户联系 ?
        
        [HiddenInput]
        public string ReturnUrl { get; set; }
    }
}