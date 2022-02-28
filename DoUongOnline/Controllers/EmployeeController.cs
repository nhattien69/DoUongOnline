using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoUongOnline.Models;
using PagedList.Mvc;
using PagedList;
using System.Net;
using System.Data.Entity;

namespace DoUongOnline.Controllers
{
    public class EmployeeController : Controller
    {
        private WebsiteCoffeeShopEntities1 db = new WebsiteCoffeeShopEntities1();
        // GET: Manager
        public ActionResult Index()
        {
            var dh = db.DonHang.Where(x => x.IdTinhTrangDH == "1").ToList();
            ViewBag.tongdhmoi = dh.Count();
            Tab();
            DateTime datenow = DateTime.Parse(DateTime.Now.ToShortDateString());
            ViewBag.title_char1 = "Biểu đồ doanh thu 10 ngày gần nhất";
            Char1(datenow.AddDays(-10), datenow);
            return View();
        }
        [HttpPost]
        public ActionResult Index(String start, String end)
        {
            var dh = db.DonHang.Where(x => x.IdTinhTrangDH == "1").ToList();
            ViewBag.tongdhmoi = dh.Count();
            if (start == "" || end == "")
                return RedirectToAction("Index", "Employee");
            Tab();
            DateTime datenow = DateTime.Parse(DateTime.Now.ToShortDateString());
            DateTime dateS = DateTime.Parse(start);
            DateTime dateE = DateTime.Parse(end);
            ViewBag.title_char1 = "Biểu đồ doanh thu từ ngày " + start + " đến ngày " + end;
            Char1(dateS, dateE);
            return View();
        }      
        public ActionResult ThongKeLoiNhuan()
        {          
            return PartialView();
        }
        [HttpPost]
        public ActionResult ThongKeLoiNhuan(String month)
        {
            DateTime monthS = DateTime.Parse(month);
            LoiNhuan(monthS);
            return PartialView();
        }
        private void Tab()
        {
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var tong = db.HoaDon.Where(t => t.NgayTaoHD <= lastDayOfMonth && t.NgayTaoHD.Year == lastDayOfMonth.Year).Sum(t => t.TongTien);
            var tiennhap = db.PhieuNhap.Where(t => t.NgayNhap <= lastDayOfMonth && t.NgayNhap.Year == lastDayOfMonth.Year).Sum(t => (double?)t.TongTienNhap) ?? 0;
            if (tong != null)
            {
                ViewBag.tien_ht = String.Format("{0:0,0}", tong);
                var loinhuan = tong - tiennhap;
                ViewBag.loinhuan1 = String.Format("{0:0,0}", loinhuan);
            }
            else
            {
                ViewBag.tien_ht = "0";
                var loinhuan = 0 - tiennhap;
                ViewBag.loinhuan1 = String.Format("{0:0,0}", loinhuan);
            }
        }      
        private void LoiNhuan(DateTime month)
        {
            DateTime date = month;
            List<Double> C1sl = new List<Double>();
            List<String> C1name = new List<String>();
            double tong = 0;        
                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var q = db.PhieuNhap.Where(t => t.NgayNhap.Month <= month.Month ).Sum(t => (double?)t.TongTienNhap) ?? 0;
                if (q == null)
                {
                    q = 0;
                }
                tong += (double)q;
                C1sl.Add((Double)q);
                C1name.Add(firstDayOfMonth.Month.ToString());           
            ViewBag.loinhuan = "Tổng doanh thu tháng" + month.ToString("MMMM") +
                "là " + String.Format("{0:0,0.00}", tong) + " VND";
            ViewBag.tong_tien_nhap = "Tổng doanh thu từ ngày" + String.Format("{0:0,0}", tong) + " VND";
            ViewBag.C1sl_1 = C1sl;          
            ViewBag.C1name_1 = C1name;
        }
        private void Char1(DateTime start, DateTime end)
        {
            List<Double> C1sl = new List<Double>();
            List<String> C1name = new List<String>();
            int num = (end - start).Days;
            double tong = 0;
            for (int i = 0; i <= num; i++)
            {
                DateTime f1 = end.AddDays(-num + i);
                DateTime f2 = f1.AddDays(1);
                var q = db.HoaDon.Where(t => t.NgayTaoHD >f1 && t.NgayTaoHD < f2).Sum(t => t.TongTien);
                if (q == null)
                    q = 0;
                tong += (double)q;
                C1sl.Add((Double)q);
                C1name.Add(f1.Day.ToString() + " / " + f1.Month.ToString());
            }
            ViewBag.tong_tien = "Tổng doanh thu từ ngày " + start.ToShortDateString() + " tới ngày " + end.ToShortDateString() + " là " + String.Format("{0:0,0}", tong) + " VND";
            ViewBag.C1sl = C1sl;
            ViewBag.C1sll = String.Format("0:0,0", C1sl);
            ViewBag.C1name = C1name;
        }

        //Danh sách loại nhân viên
        public ActionResult ListLoaiNV(int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            int pagesize = 5;
            int pagenum = (page ?? 1);
            {
                var c = db.LoaiNhanVien.OrderBy(x => x.IdLoaiNV);
                return View(c.ToPagedList(pagenum, pagesize));
            }
        }

        //Tạo loại nhân viên
        public ActionResult CreateLoaiNV()
        {        
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdLoaiNV,TenLoaiNhanVien")] LoaiNhanVien lnv)
        {                 
            if (ModelState.IsValid)
            {                
                db.LoaiNhanVien.Add(lnv);
                db.SaveChanges();
                return RedirectToAction("ListLoaiNV");
            }
            return View(lnv) ;
        }

        //Danh sách nhân viên
        public ActionResult List(int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            int pagesize = 5;
            int pagenum = (page ?? 1);
            {
                var c = db.NhanVien.OrderBy(x => x.IdNV);              
                return View(c.ToPagedList(pagenum, pagesize));
            }
        }


        //Tạo mới nhân viên
        public bool IsNameExist(string name)
        {
            var result = db.NhanVien.Any(c => c.EmailNV == name);
            return result;
        }
        public ActionResult Create()
        {
            ViewBag.IdLoaiNV = new SelectList(db.LoaiNhanVien, "IdLoaiNV", "TenLoaiNhanVien");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdNV,EmailNV,MatKhau,TenNhanVien,SDTNV,NgaySinh,DiaChi,NgayCapTK,TinhTrang,IdLoaiNV")] NhanVien nv)
        {
            if (IsNameExist(nv.EmailNV))
            {
                ModelState.AddModelError("EmailNV", "Email đã tồn tại");
            }
            if (ModelState.IsValid)
            {                     
                    nv.NgayCapTK = DateTime.Now;
                    nv.TinhTrang = true;
                    db.NhanVien.Add(nv);
                    db.SaveChanges();
                Response.Write("<script> alert('Thêm nhân viên thành công.')</script>");
                return RedirectToAction("List");          
            }
            ViewBag.IdLoaiNV = new SelectList(db.LoaiNhanVien, "IdLoaiNV", "TenLoaiNhanVien");
            return View(nv);
        }

        //Edit Nhân viên
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }          
            NhanVien nv = db.NhanVien.Find(id);
            nv.NgaySinh.ToShortDateString();
            if (nv == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdLoaiNV = new SelectList(db.LoaiNhanVien, "IdLoaiNV", "TenLoaiNhanVien", nv.IdLoaiNV);
            return View(nv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdNV,EmailNV,MatKhau,TenNhanVien,SDTNV,NgaySinh,DiaChi,NgayCapTK,TinhTrang,IdLoaiNV")] NhanVien nv)
        {
            if (ModelState.IsValid)
            {
                nv.NgaySinh.ToShortDateString();
                db.Entry(nv).State = EntityState.Modified;
                db.SaveChanges();            
                return RedirectToAction("List");
            }
            ViewBag.IdLoaiNV = new SelectList(db.LoaiNhanVien, "IdLoaiNV", "TenLoaiNhanVien", nv.IdLoaiNV);
            return View(nv);
        }

        //Đăng nhập nhân viên
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["employee"] != null)
                return RedirectToAction("Index", "Employee");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(NhanVien ut)
        {
            if (ModelState.IsValid)
            {
                var log = db.NhanVien.Where(model => model.EmailNV.Equals(ut.EmailNV) && model.MatKhau.Equals(ut.MatKhau)).FirstOrDefault();
                if (log != null)
                {                                  
                    Session["employee"] = log;
                    Session["TypeNV"] = log.IdLoaiNV;
                    Session["employeeid"] = log.IdNV;
                    return RedirectToAction("Index", "Employee");
                }
                else if (log != null && log.TinhTrang == false)
                {
                    ModelState.AddModelError("MatKhau", "Tài khoản không khả dụng");
                }
                else
                {
                    ModelState.AddModelError("MatKhau", "Vui lòng kiểm tra email hoặc mật khẩu");
                }
            }
            return View(ut);
        }

        //Đăng xuất nhân viên
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Employee");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}