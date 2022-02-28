using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoUongOnline.Models;
using PagedList.Mvc;
using PagedList;
using System.Data.Entity;
using System.Data;

namespace DoUongOnline.Controllers
{
    public class CustomerController : Controller
    {
        WebsiteCoffeeShopEntities1 db = new WebsiteCoffeeShopEntities1();
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        //Customer
        public ActionResult UpdateInfo(int id)
        {
            //int id = Convert.ToInt32(Session["customer_id"]);
            return View(db.KhachHang.Where(kh => kh.IdKH == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult UpdateInfo(int id, KhachHang KH)
        {
            db.Entry(KH).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = KH.IdKH });
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword change)
        {

            int id = Convert.ToInt32(Session["customer_id"]);
            var acc = db.KhachHang.Where(kh => kh.IdKH == id && kh.MatKhau == change.OldPassword).FirstOrDefault();

            if (acc != null && change.NewPassword == change.ConfirmPassword)
            {
                acc.MatKhau = change.NewPassword;
                acc.ConfirmPass = change.ConfirmPassword;
                db.SaveChanges();
                Response.Write("<script> alert('Thay đổi mật khẩu thành công.')</script>");
            }

            else
            {
                Response.Write("<script> alert('Vui lòng kiểm tra lại.')</script>");
            }
            return View();
        }

        // Nhắn tin với admin
        public ActionResult ChatWithAdmin()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var info = db.KhachHang.Where(kh => kh.IdKH == id).FirstOrDefault();
            return View(info);
        }

        // Đăng nhập
        public ActionResult Login()
        {
            // Kiểm tra khách hàng đã đăng nhập trước đó chưa
            if (Session["customer"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(CustomerLogin khachHang)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tình trạng
                var log = db.KhachHang.Where(model => model.EmailKH.Equals(khachHang.EmailKH) && model.MatKhau.Equals(khachHang.MatKhau)).FirstOrDefault();
                if (log != null && log.TinhTrang == false)
                {
                    ModelState.AddModelError("MatKhau", "Tài khoản không khả dụng");
                }
                // kiểm tra đăng nhập
                else if (log != null)
                {
                    // Lưu thông tin đăng nhập
                    Session["customer"] = log;
                    Session["customer_id"] = log.IdKH;
                    Session["customer_name"] = log.TenKhachHang;
                    Session["Checked_Point"] = "false";
                    return RedirectToAction("Index", "Home");
                    
                }
                else
                {
                    //ViewBag.Message = "Vui lòng kiểm tra lại email hoặc mật khẩu.";
                    ModelState.AddModelError("MatKhau", "Vui lòng kiểm tra lại email hoặc mật khẩu.");
                }
            }
            return View();
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                var check_ID = db.KhachHang.Where(kh => kh.EmailKH == khachHang.EmailKH || kh.SDTKH == khachHang.SDTKH).FirstOrDefault();
                if (check_ID == null)
                {
                    db.Configuration.ValidateOnSaveEnabled = false;
                    khachHang.NgayTaoTK = DateTime.Now;
                    khachHang.DiemTichLuy = 0;
                    khachHang.TinhTrang = true;
                    db.KhachHang.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.ErrorRegister = "Email này đã tồn tại";
                    return View();
                }
            }
            return View();
        }

        //Admin
        //Danh sách khách hàng
        public ActionResult List(int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            int pagesize = 5;
            int pagenum = (page ?? 1);
            {
                var c = db.KhachHang.OrderBy(x => x.IdKH);
                return View(c.ToPagedList(pagenum, pagesize));
            }
        }
        //sửa tình trạng khách hàng
        public ActionResult Status(int? id)
        {
            KhachHang khachHang = db.KhachHang.Find(id);
            khachHang.TinhTrang = (khachHang.TinhTrang == true) ? false : true;
            db.Entry(khachHang).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List","Customer");
        }
        //Nhắn tin với khách hàng
        public ActionResult ChatWithCustomer()
        {
            return View();
        }

       
    }
}