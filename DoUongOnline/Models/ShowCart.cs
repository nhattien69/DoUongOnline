using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoUongOnline.Models
{
    public class ShowCart
    {
        public SanPham SanPhams { get; set; }
        public Cart cart { get; set; }
    }
}