using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoUongOnline.Models;

namespace DoUongOnline.Controllers
{
    public class HomeController : Controller
    {
        WebsiteCoffeeShopEntities1 db = new WebsiteCoffeeShopEntities1();
        public ActionResult Index(string Search)
        {
            List<SanPham> sanpham = db.SanPham.ToList();
            if (Search != null)
            {
                var FindData = db.SanPham.Where(x => x.IdLoaiSP.Contains(Search)).ToList();
                if (FindData.Count == 0)
                {
                    ViewBag.Msg = "Data Not Found";
                    return View();
                }
                else
                {
                    return View(FindData);
                }
            }
            var obj = db.SanPham.ToList();
            return View(obj);
        }     
        public ActionResult ListProduct()
        {
            return View(db.SanPham.ToList());

        }

        public ActionResult Details(int id)
        {
            var sp = db.SanPham.Find(id);
            Session["hinhanhh"] = sp.HinhAnh;
            return View(db.SanPham.Where(s => s.IdSP == id).FirstOrDefault());
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult TinTuc()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}