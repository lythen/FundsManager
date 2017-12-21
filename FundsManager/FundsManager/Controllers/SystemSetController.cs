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
using FundsManager.ViewModels;
using FundsManager.Common;
namespace FundsManager.Controllers
{
    public class SystemSetController : Controller
    {
        private FundsContext db = new FundsContext();

        // GET: SystemSet
        public ActionResult Index()
        {
            return View(db.Dic_Post.ToList());
        }

        // GET: SystemSet/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dic_Post dic_Post = db.Dic_Post.Find(id);
            if (dic_Post == null)
            {
                return HttpNotFound();
            }
            return View(dic_Post);
        }

        // GET: SystemSet/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemSet/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "post_id,post_name")] Dic_Post dic_Post)
        {
            if (ModelState.IsValid)
            {
                db.Dic_Post.Add(dic_Post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dic_Post);
        }

        // GET: SystemSet/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dic_Post dic_Post = db.Dic_Post.Find(id);
            if (dic_Post == null)
            {
                return HttpNotFound();
            }
            return View(dic_Post);
        }

        // POST: SystemSet/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "post_id,post_name")] Dic_Post dic_Post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dic_Post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dic_Post);
        }

        // GET: SystemSet/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dic_Post dic_Post = db.Dic_Post.Find(id);
            if (dic_Post == null)
            {
                return HttpNotFound();
            }
            return View(dic_Post);
        }

        // POST: SystemSet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dic_Post dic_Post = db.Dic_Post.Find(id);
            db.Dic_Post.Remove(dic_Post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #region 网站设置
        public ActionResult SiteInfo()
        {
            ViewModels.SiteInfo info = FundsManager.Controllers.SiteInfo.getSiteInfo();
            return View(info);
        }
        public ActionResult SiteSet()
        {
            ViewModels.SiteInfo info = FundsManager.Controllers.SiteInfo.getSiteInfo();
            return View(info);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SiteSet([Bind(Include = "name,company,introduce,companyAddress,companyPhone,companyEmail,managerName,managerPhone,managerEmail")]ViewModels.SiteInfo info)
        {

            Sys_SiteInfo model = db.Sys_SiteInfo.First();
            if (model.site_name != info.name)
            {
                db.Sys_SiteInfo.Remove(model);
                model = info.toDBModel();
                db.Sys_SiteInfo.Add(model);
            }
            else
            {
                model = info.toDBModel();
                db.Entry<Sys_SiteInfo>(model).State = EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
                DBCaches<Sys_SiteInfo>.ClearCache("site-name");
                DBCaches<Sys_SiteInfo>.ClearCache("site-info");
            }
            catch(Exception ex)
            {
                @ViewBag.msg = "修改失败。";
            }
            @ViewBag.msg = "修改成功。";
            return View(info);
        }
        #endregion
        #region 模块设置
        public ActionResult Module()
        {
            List<ModuleInfo> models = BLL.Sys_Controller.getRoleInfo();
            foreach(ModuleInfo model in models)
            {
                int[] roles = (from rvc in db.Role_vs_Controller
                               where rvc.rvc_controller == model.name
                               select rvc.rvc_role_id
                               ).ToArray();
                List<RoleInfo> rvcs = BLL.Dic_Role.getRoleInfo();
                foreach(RoleInfo item in rvcs)
                {
                    if (roles.Contains(item.id)) item.hasrole = true;
                }
                model.roles = rvcs;
            }
            return View(models);
        }
        public ActionResult Module(List<ModuleInfo> models)
        {

            return View(models);
        }
        #endregion
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
