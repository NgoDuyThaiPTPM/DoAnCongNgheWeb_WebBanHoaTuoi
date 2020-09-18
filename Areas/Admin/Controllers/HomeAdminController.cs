using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_BANHOATUOI.Models;
using PagedList.Mvc;
using PagedList;
using System.IO;

namespace WEB_BANHOATUOI.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        //
        // GET: /Admin/HomeAdmin/
        SQLShopHoaDataContext db = new SQLShopHoaDataContext();
        public ActionResult Index()
        {
            if (Session["TKAD"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection col)
        {
            var tendn = col["uname"];
            var mk = col["pswd"];
            TAIKHOANADMIN tk = db.TAIKHOANADMINs.SingleOrDefault(n => n.TENTK == tendn && n.MK == mk);
            if (tk != null)
            {
                @Session["TKAD"] = "Xin Chào Admin: " + tk.TENTK;
                @Session["Taikhoan"] = tk;
                @Session["DX"] = "Đăng xuất";
                @Session["DN"] = "";
                return RedirectToAction("Index", "HomeAdmin");
            }
            else
            {
                ViewBag.TB = "Tên tài khoản hoặc mật khẩu không đúng";

            }
            return View("Login");
        }
        public ActionResult DangXuat()
        {
            @Session["DX"] = "";
            @Session["TKAD"] = null;
            return RedirectToAction("Index", "HomeAdmin");
        }
        public ActionResult QLHoa(int? page)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 4;
            return View(db.HOAs.ToList().OrderBy(n => n.MAHOA).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemMoiHoa()
        {
            if (Session["DX"] == "" || Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            ViewBag.MACHUDE = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TENCHUDE), "MACHUDE", "TENCHUDE");
            ViewBag.Loai = new SelectList(db.LOAIs.ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemMoiHoa(HOA hoa, HttpPostedFileBase fileUp)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            ViewBag.MACHUDE = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TENCHUDE), "MACHUDE", "TENCHUDE");
            ViewBag.Loai = new SelectList(db.LOAIs.ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI");
            if (fileUp == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileUp.FileName);
                    var path = Path.Combine(Server.MapPath("~/Hinh"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUp.SaveAs(path);
                    }
                    hoa.ANHBIA = filename;
                    // Lưu vào cơ sở dữ liệu
                    db.HOAs.InsertOnSubmit(hoa);
                    db.SubmitChanges();
                }
                return RedirectToAction("QLHoa", "HomeAdmin");
            }
        }
        [HttpGet]
        public ActionResult XoaHoa(int id)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            HOA hoa = db.HOAs.SingleOrDefault(n => n.MAHOA == id);
            ViewBag.MAHOA = hoa.MAHOA;
            if (hoa == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hoa);
        }
        [HttpPost, ActionName("XoaHoa")]
        public ActionResult XacNhanXoaHoa(int id)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            HOA hoa = db.HOAs.SingleOrDefault(n => n.MAHOA == id);
            ViewBag.MAHOA = hoa.MAHOA;
            if (hoa == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.HOAs.DeleteOnSubmit(hoa);
            db.SubmitChanges();
            return RedirectToAction("QLHoa");
        }
        [HttpGet]
        public ActionResult CapNhatHoa(int id)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            HOA hoa = db.HOAs.SingleOrDefault(n => n.MAHOA == id);
            if (hoa == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MACHUDE = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TENCHUDE), "MACHUDE", "TENCHUDE", hoa.MAHOA);
            ViewBag.Loai = new SelectList(db.LOAIs.ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI", hoa.LOAI);
            return View(hoa);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CapNhatHoa(HOA hoa, HttpPostedFileBase fileUp)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            ViewBag.MACHUDE = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TENCHUDE), "MACHUDE", "TENCHUDE");
            ViewBag.Loai = new SelectList(db.LOAIs.ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI");
            if (fileUp == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Update lại cơ sở dữ liệu
            else
            {
                if (ModelState.IsValid)
                {
                    // lưu tên file
                    var filename = Path.GetFileName(fileUp.FileName);
                    // lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/Hinh"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUp.SaveAs(path);
                    }
                    hoa.ANHBIA = filename;
                    //lưu vào CSDL
                    UpdateModel(hoa);
                    db.SubmitChanges();
                }
                return RedirectToAction("QLHoa");
            }


        }
        public ActionResult QLChuDe(int? page)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 4;
            return View(db.CHUDEs.ToList().OrderBy(n => n.MACHUDE).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemChuDe()
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemChuDe(CHUDE cd)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            if (ModelState.IsValid)
            {
                db.CHUDEs.InsertOnSubmit(cd);
                db.SubmitChanges();
            }
            // Lưu vào cơ sở dữ liệu
            return RedirectToAction("QLChuDe", "HomeAdmin");
        }
        [HttpGet]
        public ActionResult XoaChuDe(int id)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            CHUDE cd = db.CHUDEs.SingleOrDefault(n => n.MACHUDE == id);
            ViewBag.MACHUDE = cd.MACHUDE;
            if (cd == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(cd);
        }
        [HttpPost, ActionName("XoaChuDe")]
        public ActionResult XacNhanXoaCD(int id)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            CHUDE cd = db.CHUDEs.SingleOrDefault(n => n.MACHUDE == id);
            ViewBag.MACHUDE = cd.MACHUDE;
            if (cd == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.CHUDEs.DeleteOnSubmit(cd);
            db.SubmitChanges();
            return RedirectToAction("QLChuDe");
        }
        [HttpGet]
        public ActionResult CapNhatChuDe(int id)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            CHUDE cd = db.CHUDEs.SingleOrDefault(n => n.MACHUDE == id);
            if (cd == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(cd);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CapNhatChuDe(CHUDE cd)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            if (ModelState.IsValid)
            {
                UpdateModel(cd);
                db.SubmitChanges();
            }
            return RedirectToAction("QLChuDe");
        }
        public ActionResult QLLoaiHoa(int? page)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 4;
            return View(db.LOAIs.ToList().OrderBy(n => n.MALOAI).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemLoaiHoa()
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemLoaiHoa(LOAI loai)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            if (ModelState.IsValid)
            {
                db.LOAIs.InsertOnSubmit(loai);
                db.SubmitChanges();
            }
            // Lưu vào cơ sở dữ liệu
            return RedirectToAction("QLLoaiHoa", "HomeAdmin");
        }
        [HttpGet]
        public ActionResult XoaLoaiHoa(int id)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            LOAI loai = db.LOAIs.SingleOrDefault(n => n.MALOAI == id);
            ViewBag.MALOAI = loai.MALOAI;
            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loai);
        }
        [HttpPost, ActionName("XoaLoaiHoa")]
        public ActionResult XacNhanLoaiHoa(int id)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            LOAI loai = db.LOAIs.SingleOrDefault(n => n.MALOAI == id);
            ViewBag.MALOAI = loai.MALOAI;
            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.LOAIs.DeleteOnSubmit(loai);
            db.SubmitChanges();
            return RedirectToAction("QLLoaiHoa");
        }
        [HttpGet]
        public ActionResult CapNhatLoaiHoa(int id)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            LOAI loai = db.LOAIs.SingleOrDefault(n => n.MALOAI == id);
            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loai);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CapNhatLoaiHoa(LOAI loai)
        {
            if (Session["DX"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            if (ModelState.IsValid)
            {
                UpdateModel(loai);
                db.SubmitChanges();
            }
            return RedirectToAction("QLLoaiHoa");
        }
    }
}
