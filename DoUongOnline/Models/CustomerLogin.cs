using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoUongOnline.Models
{
    public class CustomerLogin
    {
        [Required(ErrorMessage = "Bạn chưa nhập email")]
        [DataType(DataType.EmailAddress)]
        public string EmailKH { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }
    }
}

