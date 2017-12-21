using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FundsManager.DAL;
using FundsManager.Models;
using System.IO;
using FundsManager.Common;
using System.Configuration;
namespace FundsManager.Controllers
{
    public class UserManagerController : Controller
    {
        private FundsContext db = new FundsContext();

        // GET: UserManager
        public ActionResult Index()
        {
            return View(db.User_Info.ToList());
        }

        // GET: UserManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Info user_Info = db.User_Info.Find(id);
            if (user_Info == null)
            {
                return HttpNotFound();
            }
            return View(user_Info);
        }

        // GET: UserManager/Create
        public ActionResult Create()
        {
            setSelect();
            return View();
        }

        // POST: UserManager/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "user_id,user_name,real_name,user_certificate_type,user_certificate_no,user_mobile,user_email,user_password,user_salt,user_state,user_login_times")] User_Info user_Info)
        {
            setSelect();
            if (ModelState.IsValid)
            {
                db.User_Info.Add(user_Info);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user_Info);
        }

        // GET: UserManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Info user_Info = db.User_Info.Find(id);
            if (user_Info == null)
            {
                return HttpNotFound();
            }
            return View(user_Info);
        }

        // POST: UserManager/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,user_name,real_name,user_certificate_type,user_certificate_no,user_mobile,user_email,user_password,user_salt,user_state,user_login_times")] User_Info user_Info)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user_Info).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user_Info);
        }

        // GET: UserManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Info user_Info = db.User_Info.Find(id);
            if (user_Info == null)
            {
                return HttpNotFound();
            }
            return View(user_Info);
        }

        // POST: UserManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User_Info user_Info = db.User_Info.Find(id);
            db.User_Info.Remove(user_Info);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public JsonResult UploadPicture()
        {
            ViewModels.BaseJsonData json = new ViewModels.BaseJsonData();
            var file = Request.Files["data"];
            if (file == null)
            {
                json.state = 0;
                json.msg_text = "没有文件，请重新上传。";
            }
            if (Path.GetExtension(file.FileName).ToLower() != ".jpg")
            {
                json.state = 0;
                json.msg_text = "请上传jpg格式文件。";
            }
            string photoTempDir= ConfigurationManager.AppSettings["tempPhotoPath"];
            if (!Directory.Exists(photoTempDir)) Directory.CreateDirectory(photoTempDir);
            string guid = Guid.NewGuid().ToString("N");
            string file_name = string.Format("{0}{1}.jpg", photoTempDir, guid);
            string file_name_temp = string.Format("{0}{1}_temp.jpg",photoTempDir,guid);
            file.SaveAs(file_name);
            ImageFun.MakeThumbnail(file_name, file_name_temp, 200, 0, "W");
            json.state = 1;
            json.data = Path.GetFileName(file_name_temp); ;
            return Json(json);
        }
        public void setSelect()
        {
            DBCaches<User_Info>.ClearAllCache();
            List<SelectOption> options = DropDownList.getDepartment();
            ViewBag.Department = DropDownList.SetDropDownList(options);
            options = DropDownList.PostSelect();
            ViewBag.Post = DropDownList.SetDropDownList(options);
            options = DropDownList.SexSelect();
            ViewBag.Sex = DropDownList.SetDropDownList(options);
            options = DropDownList.UserStateSelect();
            ViewBag.State = DropDownList.SetDropDownList(options);
            options = DropDownList.CardTypeSelect();
            ViewBag.CardType = DropDownList.SetDropDownList(options);
            options = DropDownList.RoleSelect();
            ViewBag.Role = DropDownList.SetDropDownList(options);
        }
        [HttpPost]
        public JsonResult GetPost(string id)
        {
            List<SelectOption> options = DropDownList.getDepartment(PageValidate.FilterParam(id));
            return Json(options, JsonRequestBehavior.AllowGet);
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
