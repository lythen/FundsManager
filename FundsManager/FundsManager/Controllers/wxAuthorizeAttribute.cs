using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lythen.DAL;
using Lythen.Common;
namespace Lythen.Controllers
{
    public class wxAuthorizeAttribute: AuthorizeAttribute
    {
        private FundsContext db = new FundsContext();
        public wxAuthorizeAttribute() {
        }
        /// <summary>
        /// 对应Action允许的角色
        /// </summary>
        public new string Roles { get; set; }
        /// <summary>
        /// 在请求授权时调用
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string[] AuthRoles;
            if(string.IsNullOrEmpty(Roles)) AuthRoles =new string[]{ "系统管理员" };
            else AuthRoles = Roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (httpContext == null)
            {
                throw new ArgumentNullException("HttpContext");
            }
            if (AuthRoles == null || AuthRoles.Length == 0)
            {
                return false;
            }
            #region 确定当前用户角色是否属于指定的角色
            //获取当前用户所在角色
            int userid = PageValidate.FilterParam(httpContext.User.Identity.Name);
            string[] userRoles;
            string cache_key = "user_vs_roles-" + userid;
            object objUVR = DataCache.GetCache(cache_key);
            if (objUVR == null)
            {
                userRoles = (from u in db.User_Info
                             join uvr in db.User_vs_Role
                             on u.user_id equals uvr.uvr_user_id
                             join r in db.Dic_Role
                             on uvr.uvr_role_id equals r.role_id
                             where u.user_id == userid
                             select r.role_name
                                 ).ToArray();
                if (userRoles.Count() == 0) return false;
                DataCache.SetCache(cache_key, userRoles);
            }
            else userRoles = (string[])objUVR;
            
            //验证是否属于对应角色
            for (int i = 0; i < AuthRoles.Length; i++)
            {
                if (userRoles.Contains(AuthRoles[i]))
                {
                    return true;
                }
            }
            #endregion
            return false;
        }
        /// <summary>
        /// 提供一个入口点用于自定义授权检查，通过为true
        /// </summary>
        /// <param name="filterContext"></param>
        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    object objList = DataCache.GetCache("roles_controller");
        //    string[] roles;
        //    if (objList == null)
        //    {
        //        roles = (from rs in db.Sys_Roles
        //                 select rs.role_name).ToArray();
        //        DataCache.SetCache("roles_controller", roles);
        //    }
        //    else roles = (string[])objList;

        //    if (roles.Count() == 0) this.AuthRoles = new string[] { };
        //    else this.AuthRoles = roles.ToArray();
        //    //string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
        //    //string actionName = filterContext.ActionDescriptor.ActionName;
        //    ////获取config文件配置的action允许的角色，以后可以转到数据库中
        //    //string roles = GetActionRoles(actionName, controllerName);
        //    //if (!string.IsNullOrWhiteSpace(roles))
        //    //{
        //    //    this.AuthRoles = roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        //    //}
        //    //else
        //    //{
        //    //    this.AuthRoles = new string[] { };
        //    //}
        //    base.OnAuthorization(filterContext);
        //}
    }
}