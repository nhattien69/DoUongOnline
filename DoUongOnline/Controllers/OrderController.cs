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
using System.Globalization;

namespace DoUongOnline.Controllers
{
    public class OrderController : Controller
    {
        private WebsiteCoffeeShopEntities1 db = new WebsiteCoffeeShopEntities1();
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        //Customer
        public ActionResult MyOrdersList()
        {
            int idKH = Convert.ToInt32(Session["customer_id"]);
            var MyOrdersList = db.DonHang.Where(dh => dh.IdKH == idKH).OrderByDescending(dh => dh.NgayTaoDon).ToList(); //Hiển thị danh sách được sắp xếp theo ngày tạo đơn (mới -> cũ)

            return View(MyOrdersList);
        }

        public ActionResult OrderDetail(int id)
        {
            Session["idDH"] = id;
            return View(db.ChiTietDonHang.Where(s => s.IdDH == id).ToList());
        }
        [HttpPost]
        public ActionResult CancelOrder(int id, DonHang donhang)
        {
            donhang = db.DonHang.Where(s => s.IdDH == id).FirstOrDefault();
            if (donhang.IdTinhTrangDH == "1")
            {
                donhang.IdTinhTrangDH = "6";

                // Cập nhật lại số lượng khi khách hàng hủy đơn
                foreach (var item in donhang.ChiTietDonHang)
                {
                    var sp = db.SanPham.Where(s => s.IdSP == item.IdSP).FirstOrDefault();
                    sp.SoLuongCon += item.SoLuong;
                }

                // Cập nhật lại điểm cho khách hàng
                int idKH = Convert.ToInt32(Session["customer_id"]);
                var khachhang = db.KhachHang.Where(kh => kh.IdKH == idKH).FirstOrDefault();
                khachhang.DiemTichLuy -= (int)(donhang.TongTien * 0.02);
                khachhang.ConfirmPass = khachhang.MatKhau;
                db.SaveChanges();
                return RedirectToAction("MyOrdersList");
            }
            else
            {
                TempData["Message_CancelOrder"] = "Bạn không được phép hủy đơn hàng này";
                return RedirectToAction("OrderDetail", new { id = id });
            }
        }

        //Admin
        //Danh sách đơn hàng
        public ActionResult List(string searchBy, string search, int? page, double min = double.MinValue, double max = double.MaxValue)
        {

            if (searchBy == "LoaiTinhTrang")
            {
                int pagesize = 10;
                int pagenum = (page ?? 1);
                {
                    var c = db.DonHang.OrderBy(x => x.IdTinhTrangDH).Where(x => x.TinhTrangDonHang.TenTinhTrangDH.Contains(search) || search == null).ToList();
                    return View(c.ToPagedList(pagenum, pagesize));
                }
            }
            else
            {
                int pagesize = 10;
                int pagenum = (page ?? 1);
                {
                    var c = db.DonHang.OrderBy(x => x.IdTinhTrangDH).Where(x => x.KhachHang.TenKhachHang.Contains(search) || search == null).ToList();                   
                    return View(c.ToPagedList(pagenum, pagesize));
                }
            }
        }
        [HttpPost]
        public ActionResult List(String beginDate,String endDate,string searchBy, string search, int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            
            int pagesize = 10;
            int pagenum = (page ?? 1);           
            DateTime beginDate1 = DateTime.Parse(beginDate);          
            endDate = beginDate1.AddDays(1).ToString("MM/dd/yyyy");
            System.Diagnostics.Debug.WriteLine("your message here " + beginDate);
            List<DonHang> dsdh = new List<DonHang>();
            String query = "select * from DonHang";
            if (!beginDate.Equals(""))
                query += " where NgayTaoDon >= '" + beginDate + "'";
            if (!beginDate.Equals(""))
                query += " and NgayTaoDon <= '" + endDate + "'";

            dsdh = db.DonHang.SqlQuery(query).ToList();
                      
            return View(dsdh.ToPagedList(pagenum,pagesize));
        }


        //Chi tiết đơn hàng
        public ActionResult DetailList(int? id)
        {
            var dh = db.DonHang.Find(id);
            ViewBag.diachi = dh.DiaChiGiaoHang;
            ViewBag.tenkh = dh.KhachHang.TenKhachHang;
            ViewBag.sdtkh = dh.KhachHang.SDTKH;
            string tongtien = dh.TongTien.ToString("#,##");
            ViewBag.tongtien1 = tongtien;
            ViewBag.madh = dh.IdDH;          
            return View(db.ChiTietDonHang.Where(s => s.IdDH == id).ToList());
        }

        //Tình trạng đơn hàng     
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang dh = db.DonHang.Find(id);
            Session["iddh"] = dh.IdDH;
            //Session["diachi"] = dh.DiaChiGiaoHang;
            if (dh == null)
            {
                return HttpNotFound();
            }          
            ViewBag.IdTinhTrangDH = new SelectList(db.TinhTrangDonHang, "IdTinhTrangDH", "TenTinhTrangDH", dh.IdTinhTrangDH);
            return View(dh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdDH,NgayTaoDon,DiaChiGiaoHang,GhiChu,TongTien,DiemKHSuDung,IdKH,IdTinhTrangDH,NgayGioCapNhat")] DonHang dh,int? id)
        {
            if (ModelState.IsValid)
            {            
                dh.NgayGioCapNhat = DateTime.Now;             
                if (dh.IdTinhTrangDH != "6")
                {               
                    db.Entry(dh).State = EntityState.Modified;
                    db.SaveChanges();                                       
                    if (dh.IdTinhTrangDH == "5")
                    {
                        HoaDon hd = new HoaDon();
                        hd.IdDH = dh.IdDH;
                        hd.IdNV = (int)Session["employeeid"];
                        hd.NgayTaoHD = DateTime.Now;
                        hd.TongTien = dh.TongTien;
                        db.HoaDon.Add(hd);
                        db.SaveChanges();
                    }
                    else
                    {
                        var c = db.HoaDon.Where(x => x.IdDH == dh.IdDH).FirstOrDefault();
                        if (c != null)
                        {
                            HoaDon hd = c;
                            db.HoaDon.Remove(hd);
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("List");
                }
                else if(dh.IdTinhTrangDH == "6")
                {
                    if (Session["TypeNV"].ToString() == "QL")
                    {
                        db.Entry(dh).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("List");
                    }
                    else
                    {                     
                        Response.Write("<script> alert('Nhân viên không có quyền hủy đơn hàng!')</script>");
                    }
                }
                            
            }
            ViewBag.IdTinhTrangDH = new SelectList(db.TinhTrangDonHang, "IdTinhTrangDH", "TenTinhTrangDH", dh.IdTinhTrangDH);
            return View(dh);
        }
          
    }
}