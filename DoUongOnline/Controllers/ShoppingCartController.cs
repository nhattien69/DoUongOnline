using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoUongOnline.Models;

namespace DoUongOnline.Controllers
{
    public class ShoppingCartController : Controller
    {
        WebsiteCoffeeShopEntities1 database = new WebsiteCoffeeShopEntities1();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowCart()
        {
            if(Session["Cart"] == null)
            {
                return View("EmptyCart");
            }
            Cart _cart = Session["Cart"] as Cart;
            return View(_cart);
        }

        // Action tạo mới giỏ hàng
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // Action thêm sản phẩm vào giỏ hàng
        public ActionResult AddToCart(int id)
        {
            var _sp = database.SanPham.SingleOrDefault(s => s.IdSP == id); // Lấy sản phẩm theo id

            if (_sp != null)
            {
                if (_sp.IdKM == null)
                {
                    _sp.GiaBan = _sp.GiaBan;
                }
                else
                {
                    _sp.GiaBan = _sp.GiaBan - ((_sp.GiaBan * _sp.KhuyenMai.PhanTramKM) / 100);
                }
                GetCart().Add_Product_Cart(_sp);
            }
            return RedirectToAction("ShowCart", "ShoppingCart");
        }

        // Cập nhật số lượng sản phẩm và tính lại tổng tiền
        public ActionResult Update_Cart_Quantity(FormCollection form)
        {
            Cart cart = Session["Cart"] as Cart;
            int id_sp = int.Parse(form["IdSP"]);
            int _quantity = int.Parse(form["cartQuantity"]);
            cart.Update_quantity(id_sp, _quantity);
            return RedirectToAction("ShowCart", "ShoppingCart");
        }

        // Xóa dòng sản phẩm trong giỏ hàng
        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove_CartIem(id);
            return RedirectToAction("ShowCart", "ShoppingCart");
        }

        // Lấy tổng số lượng sản phẩm có trong giỏ hàng
        public PartialViewResult BagCart()
        {
            int total_quantity_item = 0;
            Cart cart = Session["Cart"] as Cart;
            if(cart != null)
            {
                total_quantity_item = cart.Total_quantity();
            }
            ViewBag.QuantityCart = total_quantity_item;
            return PartialView("BagCart");
        }
    }
}