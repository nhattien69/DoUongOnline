using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoUongOnline.Models
{
    public class CartItem
    {
        public SanPham _sanpham { get; set; }
        public int _quantity { get; set; }
    }
    public class Cart
    {
        List<CartItem> items = new List<CartItem>();

        public IEnumerable<CartItem> Items
        {
            get { return items; }
        }

        //Lấy sản phẩm bỏ vào giỏ hàng
        public void Add_Product_Cart(SanPham _sp, int _quan = 1)
        {
            var item = Items.FirstOrDefault(s => s._sanpham.IdSP == _sp.IdSP);

            if (item == null)
            {
                items.Add(new CartItem
                {
                    _sanpham = _sp,
                    _quantity = _quan
                });
            }
            else
            {
                item._quantity += _quan;
            }
        }

        // Tính tổng số lượng trong giỏ hàng
        public int Total_quantity()
        {
            return items.Sum(s => s._quantity);
        }

        // Tính thành tiền cho mỗi dòng sản phẩm trong giỏ hàng
        public decimal Total_money()
        {
            var total = items.Sum(s => s._quantity * s._sanpham.GiaBan);
            return (decimal)total;
        }

        // Cập nhật lại số lượng sản phẩm ở mỗi dòng sản phẩm khi khách hàng muốn đặt mua thêm
        public void Update_quantity(int id, int _new_quan)
        {
            var item = items.Find(s => s._sanpham.IdSP == id);
            if (item != null)
            {
                if (items.Find(s => s._sanpham.SoLuongCon >= _new_quan) != null)
                {
                    item._quantity = _new_quan;
                }
                else
                {
                    item._quantity = (int)item._sanpham.SoLuongCon;
                }

            }
        }

        // Xóa sản phẩm trong giỏ hàng
        public void Remove_CartIem(int id)
        {
            items.RemoveAll(s => s._sanpham.IdSP == id);
        }

        // Xóa số lượng giỏ hàng sau khi khách hàng thực hiện thanh toán
        public void ClearCart()
        {
            items.Clear();
        }
    }
}