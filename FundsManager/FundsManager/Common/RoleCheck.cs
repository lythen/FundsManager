using FundsManager.DAL;
using System.Linq;
using System.Web;

namespace FundsManager.Common
{
    public static class RoleCheck
    {
        public static bool CheckIsAdmin(int userid)
        {
            string[] AuthRoles= { "系统管理员" };
            #region 确定当前用户角色是否属于指定的角色
            //获取当前用户所在角色
            string[] userRoles;
            string cache_key = "user_vs_roles-" + userid;
            object objUVR = DataCache.GetCache(cache_key);
            if (objUVR == null)
            {
                FundsContext db = new FundsContext();
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
        public static bool CheckIsRespond(int userid)
        {
            string[] AuthRoles = { "系统管理员","批复用户" };
            #region 确定当前用户角色是否属于指定的角色
            //获取当前用户所在角色
            string[] userRoles;
            string cache_key = "user_vs_roles-" + userid;
            object objUVR = DataCache.GetCache(cache_key);
            if (objUVR == null)
            {
                FundsContext db = new FundsContext();
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
    }
}