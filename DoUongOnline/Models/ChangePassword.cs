using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoUongOnline.Models
{
    public class ChangePassword
    {
        [DisplayName("Nhập mật khẩu cũ")]
        public string OldPassword { get; set; }

        [DisplayName("Nhập mật khẩu mới")]
        public string NewPassword { get; set; }

        [NotMapped]
        [Compare("NewPassword")]
        [DisplayName("Nhập lại mật khẩu mới")]
        public string ConfirmPassword { get; set; }
    }
}