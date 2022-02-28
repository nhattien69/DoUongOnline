using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoUongOnline.Models;
using PagedList.Mvc;
using PagedList;
using System.Net;

namespace DoUongOnline.Controllers
{
    public class InvoiceController : Controller
    {
        private WebsiteCoffeeShopEntities1 db = new WebsiteCoffeeShopEntities1();
        // GET: Invoice
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List( int? page, double min = double.MinValue, double max = double.MaxValue)
        {          
             int pagesize = 10;
             int pagenum = (page ?? 1);
             {
                 var c = db.HoaDon.OrderBy(x => x.IdDH);                       
                 return View(c.ToPagedList(pagenum, pagesize));
             }
                
        }
        [HttpPost]
        public ActionResult List(String beginDate, String endDate, string searchBy, string search, int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            int pagesize = 10;
            int pagenum = (page ?? 1);
            DateTime beginDate1 = DateTime.Parse(endDate);
            endDate = beginDate1.AddDays(1).ToString("MM/dd/yyyy");
            System.Diagnostics.Debug.WriteLine("your message here " + beginDate);
            List<HoaDon> dsdh = new List<HoaDon>();
            String query = "select * from HoaDon";
            if (!beginDate.Equals(""))
                query += " where NgayTaoHD >= '" + beginDate + "'";
            if (!endDate.Equals(""))
                query += " and NgayTaoHD <= '" + endDate + "'";

            dsdh = db.HoaDon.SqlQuery(query).ToList();
           
            return View(dsdh.ToPagedList(pagenum, pagesize));
        }
        public ActionResult DetailList(int? id)
        {
            var dh = db.DonHang.Find(id);
            ViewBag.diachi1 = dh.DiaChiGiaoHang;
            ViewBag.tenkh1 = dh.KhachHang.TenKhachHang;
            string tongtien = dh.TongTien.ToString("#,##");
            ViewBag.tongtien = tongtien;
            var hd = db.HoaDon.Where(x => x.IdDH == id).FirstOrDefault();
            ViewBag.tennv = hd.NhanVien.TenNhanVien;
            DateTime? dt = hd.NgayTaoHD;
            ViewBag.mahd = hd.IdHD;
            ViewBag.ngaytaohd = dt.Value.ToShortDateString();
            return View(db.ChiTietDonHang.Where(s => s.IdDH == id).ToList());
        }

    }
}