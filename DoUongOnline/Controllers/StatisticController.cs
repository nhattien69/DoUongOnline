using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoUongOnline.Models;
using PagedList.Mvc;
using PagedList;

namespace DoUongOnline.Controllers
{
    public class StatisticController : Controller
    {
        WebsiteCoffeeShopEntities1 db = new WebsiteCoffeeShopEntities1();
        // GET: Statistic
        public ActionResult Index()
        {
            
            return View();
          
        }
        public ActionResult ThongKeSanPham()
        {
           
            List<ChiTietDonHang> ct = db.ChiTietDonHang.ToList();
            List<SanPham> sp = db.SanPham.ToList();
            var query = from c in ct
                        join s in sp on c.IdSP equals s.IdSP into tbl                     
                        group c by new
                        {
                            IdSP = c.IdSP,
                            nameSP = c.SanPham.TenSP,
                            imageSP = c.SanPham.HinhAnh,
                            priceSP = c.SanPham.GiaBan
                        } into gr
                        orderby gr.Sum(s => s.SoLuong) descending
                        select new ViewModel
                        {
                            IdSP = gr.Key.IdSP,
                            NameSP = gr.Key.nameSP,
                            ImageSP = gr.Key.imageSP,
                            priceSP = (decimal)gr.Key.priceSP,
                            Sum_Quantity = gr.Sum(s => s.SoLuong)
                        };
            Char3();
            return View(query.Take(5).ToList());
        }
        public ActionResult SanPhamTrongNgay(int? page, double min = double.MinValue, double max = double.MaxValue)
        {
         
            int pagesize = 8;
            int pagenum = (page ?? 1);
            List<ChiTietDonHang> ct = db.ChiTietDonHang.ToList();
            List<SanPham> sp = db.SanPham.ToList();
            var query = from c in ct
                        join s in sp on c.IdSP equals s.IdSP 
                        group c by new
                        {
                            IdSP = c.IdSP,
                            nameSP = c.SanPham.TenSP,
                            imageSP = c.SanPham.HinhAnh,
                            priceSP = c.SanPham.GiaBan
                        } into gr
                        orderby gr.Sum(s => s.SoLuong) descending
                        select new ViewModel
                        {
                            IdSP = gr.Key.IdSP,
                            NameSP = gr.Key.nameSP,
                            ImageSP = gr.Key.imageSP,
                            priceSP = (decimal)gr.Key.priceSP,
                            Sum_Quantity = gr.Sum(s => s.SoLuong)
                        };
            return PartialView(query.Take(0).ToPagedList(pagenum, pagesize));
        }
        [HttpPost]
        public ActionResult SanPhamTrongNgay(String beginDate, String endDate, int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            DateTime beginDate1 = DateTime.Parse(beginDate);          
            DateTime endDate1 = beginDate1.AddDays(1);
            System.Diagnostics.Debug.WriteLine("your message here " + beginDate);

            //List<ChiTietDonHang> dsdh = new List<ChiTietDonHang>();
            //String query1 = "Select p.IdChiTietDH,p.GhiChuChiTiet,p.GiaBan,p.DanhGiaSanPham,p.IdSP,p.IdDH,p.IdSizeSP ,s.TenSP ,t.SizeSP, p.SoLuong  from ChiTietDonHang as p , SanPham as s , SizeSanPham as t, DonHang as d" +
            //    " where p.IdSP = s.IdSP and p.IdSizeSP = t.IdSizeSP and p.IdDH = d.IdDH";
            //if (!beginDate.Equals(""))
            //    query1 += " and d.NgayGioCapNhat >= '" + beginDate + "'";
            //if (!beginDate.Equals(""))
            //    query1 += " and d.NgayGioCapNhat <= '" + endDate + "'";
            //dsdh = db.ChiTietDonHang.SqlQuery(query1).ToList();
            int pagesize = 8;
            int pagenum = (page ?? 1);
            List<ChiTietDonHang> ct = db.ChiTietDonHang.ToList();
            List<SanPham> sp = db.SanPham.ToList();
            var query = from c in ct
                        join s in sp on c.IdSP equals s.IdSP 
                        where c.DonHang.NgayGioCapNhat >= beginDate1 && c.DonHang.NgayGioCapNhat <= endDate1
                        group c by new
                        {
                            IdSP = c.IdSP,
                            nameSP = c.SanPham.TenSP,
                            imageSP = c.SanPham.HinhAnh,
                            priceSP = c.SanPham.GiaBan,
                            SizeSP = c.SizeSanPham.SizeSP
                        } into gr
                        orderby gr.Sum(s => s.IdSP) 
                        select new ViewModel
                        {
                            IdSP = gr.Key.IdSP,
                            NameSP = gr.Key.nameSP,
                            ImageSP = gr.Key.imageSP,
                            priceSP = (decimal)gr.Key.priceSP,
                            SizeSP = gr.Key.SizeSP,
                            Sum_Quantity = gr.Sum(s => s.SoLuong)
                        };          
            return PartialView(query.ToPagedList(pagenum, pagesize));
        }
       
        public ActionResult ThongKeNguoiDung()
        {
            List<string> u = new List<string>();
            List<int> a = new List<int>();
            var user = db.DonHang.Select(x => x.KhachHang.TenKhachHang).Distinct();
            foreach (var item in user)
            {
                a.Add(db.DonHang.Count(x => x.KhachHang.TenKhachHang == item));           
            }
            var rep = a;
            ViewBag.User = user.ToList().Take(10);
            ViewBag.Rep = a.ToList();
            return View();
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
                var q = db.HoaDon.Where(t => t.NgayTaoHD < f2).Sum(t => t.TongTien);
                if (q == null)
                    q = 0;
                tong += (double)q;
                C1sl.Add((Double)q);
                C1name.Add(f1.Day.ToString() + " / " + f1.Month.ToString());
            }
            ViewBag.tong_tien = "Tổng doanh thu từ ngày " + start.ToShortDateString() + " tới ngày " + end.ToShortDateString() + " là " + String.Format("{0:0,0.00}", tong) + " VND";
            ViewBag.C1sl = C1sl;
            ViewBag.C1name = C1name;

        }
        private void Char3()
        {
            var s = db.ChiTietDonHang.GroupBy(t => t.SanPham.TenSP).Select(t => new { TenSP = t.Key, total = t.Sum(i => i.SoLuong) }).Take(5);
            List<String> name = new List<String>();
            List<int> total = new List<int>();
            foreach (var group in s)
            {
                System.Diagnostics.Debug.WriteLine("Ma Dv: " + group.TenSP + " | SL: " + group.total);
                name.Add((String)group.TenSP);
                total.Add((int)group.total);
            }
            ViewBag.name = name;
            ViewBag.total = total;
        }
        public ActionResult LoiNhuan()
        {
          
            return View();
        }
        [HttpPost]
        public ActionResult LoiNhuan(String month)
        {
            if (!month.Equals(""))
            {
                List<PhieuNhap> dsdh = new List<PhieuNhap>();
                String query = "Select * from PhieuNhap";       
                DateTime beginDate1 = DateTime.Parse(month);
                    month = beginDate1.ToString("MM");
                    ViewBag.month = beginDate1.ToString("MM/yyyy");
                    query += " where Month(NgayNhap)  <= '" + month + "'";
                    dsdh = db.PhieuNhap.SqlQuery(query).ToList();
                    double tong = 0;
                    var q = dsdh.Sum(t => t.TongTienNhap);
                    tong += (double)q;
                   
                List<HoaDon> dshd = new List<HoaDon>();
                String query1 = "Select * from HoaDon";            
                    month = beginDate1.ToString("MM");
                    ViewBag.month2 = beginDate1.ToString("MM/yyyy");
                    query1 += " where Month(NgayTaoHD)  <= '" + month + "'";
                    dshd = db.HoaDon.SqlQuery(query1).ToList();
                    double tong1 = 0;
                    var q1 = dshd.Sum(t => t.TongTien);
                    tong1 += (double)q1;
                double tong2 = tong1 - tong;
                ViewBag.loinhuan = String.Format("{0:0,0}", tong2);
            }       
            else
            {
                ViewBag.month = "";
            }
            return View();
        }
        public ActionResult PN(int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            int pagesize = 10;
            int pagenum = (page ?? 1);
            {
                var c = db.PhieuNhap.OrderBy(x => x.IdPN).Take(0);
                return PartialView(c.ToPagedList(pagenum, pagesize));
            }
        }
        [HttpPost]
        public ActionResult PN(String month, int? page, double min = double.MinValue, double max = double.MaxValue)
        {

            int pagesize = 5;
            int pagenum = (page ?? 1);
            
            List<PhieuNhap> dsdh = new List<PhieuNhap>();
            String query = "Select * from PhieuNhap";
            if (!month.Equals(""))
            {
                DateTime beginDate1 = DateTime.Parse(month);
                month = beginDate1.ToString("MM");
                ViewBag.month1 = beginDate1.ToString("MM/yyyy");
                query += " where Month(NgayNhap)  <= '" + month + "'";
                dsdh = db.PhieuNhap.SqlQuery(query).ToList();
                double tong = 0;
                var q = dsdh.Sum(t => t.TongTienNhap);
                tong += (double)q;
                //Session["PN"] = tong;
                ViewBag.tienvon = String.Format("{0:0,0}", tong);
            }
            else
            {
                //Session["PN"] = 0;
                ViewBag.tienvon = "0";
            }
            return PartialView(dsdh.ToPagedList(pagenum, pagesize));
        }
        public ActionResult DT(int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            int pagesize = 10;
            int pagenum = (page ?? 1);
            {
                var c = db.HoaDon.OrderBy(x => x.IdHD).Take(0);
                return PartialView(c.ToPagedList(pagenum, pagesize));
            }
        }
        [HttpPost]
        public ActionResult DT(String month, int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            int pagesize = 5;
            int pagenum = (page ?? 1);
           
            List<HoaDon> dsdh = new List<HoaDon>();
            String query = "Select * from HoaDon";
            if (!month.Equals(""))
            {
                DateTime beginDate1 = DateTime.Parse(month);
                month = beginDate1.ToString("MM");
                ViewBag.month2 = beginDate1.ToString("MM/yyyy");
                query += " where Month(NgayTaoHD)  <= '" + month + "'";
                dsdh = db.HoaDon.SqlQuery(query).ToList();
                double tong = 0;
                var q = dsdh.Sum(t => t.TongTien);
                tong += (double)q;
                //Session["DT"] = tong;
                ViewBag.doanhthu = String.Format("{0:0,0}", tong);
            }
            else
            {
                //Session["DT"] = 0;
                ViewBag.doanhthu = "0";
            }
            return PartialView(dsdh.ToPagedList(pagenum, pagesize));
        }
    }
}