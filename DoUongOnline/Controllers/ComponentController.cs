using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoUongOnline.Models;
using PagedList.Mvc;
using PagedList;
using System.Net;
using Microsoft.Ajax.Utilities;

namespace DoUongOnline.Controllers
{
    public class ComponentController : Controller
    {
        private WebsiteCoffeeShopEntities1 db = new WebsiteCoffeeShopEntities1();
        // GET: Component
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ListNL(int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            
            int pagesize = 5;
            int pagenum = (page ?? 1);
            {
                var c = db.Kho.OrderBy(x => x.ChiTietPhieuNhap.IdCTPN);
                return View(c.ToPagedList(pagenum, pagesize));
            }
        }

        //Danh sách phiếu nhập
        public ActionResult ListPN(int? page, double min = double.MinValue, double max = double.MaxValue)
        {        

            int pagesize = 10;
            int pagenum = (page ?? 1);
            {              
               var c = db.PhieuNhap.OrderBy(x => x.IdPN);               
               return View(c.ToPagedList(pagenum, pagesize));
            }
        }
   
        //Tạo phiếu nhập      
        public ActionResult CreatePN()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePN([Bind(Include = "IdPN,NgayNhap,TongTienNhap,IdNV")] PhieuNhap pn)
        {
            if (ModelState.IsValid)
            {
                pn.IdNV = (int)Session["employeeid"];
                pn.NgayNhap = DateTime.Now;
                pn.TongTienNhap = 0;
                db.PhieuNhap.Add(pn);
                db.SaveChanges();
                return RedirectToAction("ListPN");
            }
            return PartialView(pn);
        }
    

      
        //Chi tiết phiếu nhập
        public ActionResult ListCTPN(int? id,int? page, double min = double.MinValue, double max = double.MaxValue)
        {

            var sp = db.PhieuNhap.Find(id);
            Session["IdPN"] = sp.IdPN;
            ViewBag.IdPN = sp.IdPN;         
            int pagesize = 10;
            int pagenum = (page ?? 1);
            {
                var c = db.ChiTietPhieuNhap.OrderBy(x => x.IdNL).Where(x=> x.IdPN == id);                        
                return View(c.ToPagedList(pagenum, pagesize));
            }
        }

        //Tạo chi tiết phiếu nhập      
        public ActionResult CreateCTPN()
        {
            ChiTietPhieuNhap ctpn = new ChiTietPhieuNhap();
            ViewBag.IdNL = new SelectList(db.NguyenLieu, "IdNL", "TenNL");         
            return View(ctpn);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCTPN(ChiTietPhieuNhap ctpn,int id)
        {
            if (ModelState.IsValid)
            {
                NguyenLieu nl = db.NguyenLieu.Where(x => x.IdNL == ctpn.IdNL).FirstOrDefault();
                ctpn.GiaNhap = nl.GiaNL * ctpn.SoLuongNhap;
                ctpn.IdPN = id;
                var pn = db.PhieuNhap.Where(x => x.IdPN == id).FirstOrDefault();              
                pn.TongTienNhap = ctpn.GiaNhap + pn.TongTienNhap;
                Kho k = new Kho();
                k.IdCTPN = ctpn.IdCTPN;
                k.SoLuong = 0;
                k.SoLuong = ctpn.SoLuongNhap;
                db.Kho.Add(k);              
                db.ChiTietPhieuNhap.Add(ctpn);
                db.SaveChanges();
                return RedirectToAction("ListCTPN", new { id = id });
              
            }
            ViewBag.IdNL = new SelectList(db.NguyenLieu, "IdNL", "TenNL");
            return View(ctpn);
        }

        public bool IsExsict(int id)
        {
            var c = db.Kho.Any(a => a.ChiTietPhieuNhap.IdNL == id);
            return c;
        }

        //Danh sách kho
        public ActionResult ListKho(int? id, int? page, double min = double.MinValue, double max = double.MaxValue)
        {
         
         
            List<Kho> kho = db.Kho.ToList();
            List<NguyenLieu> nl = db.NguyenLieu.ToList();          
            var query = from c in kho
                        join s in nl on c.ChiTietPhieuNhap.IdNL equals s.IdNL
                        group c by new
                        {
                            IdNL = c.ChiTietPhieuNhap.IdNL,
                            nameNL = c.ChiTietPhieuNhap.NguyenLieu.TenNL,
                            donvi = c.ChiTietPhieuNhap.NguyenLieu.DonVi
                        } into gr                                    
                        orderby gr.Sum(s => s.ChiTietPhieuNhap.IdNL)                     
                        select new ViewModelNL
                        {                       
                            IdNL = gr.Key.IdNL,
                            nameNL = gr.Key.nameNL,
                            donvi = gr.Key.donvi,
                            SoLuong = gr.Sum(s => s.ChiTietPhieuNhap.SoLuongNhap)
                            
                        };
            return View(query.ToList());                
        }

    }
}