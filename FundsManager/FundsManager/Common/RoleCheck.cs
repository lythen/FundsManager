using Lythen.DAL;
using System.Linq;

namespace Lythen.Common
{
    public static class RoleCheck
    {
        public static bool CheckHasAuthority(int userid, FundsContext db,params string[] auths)
        {
            if (CheckIsSuperAdmin(userid, db)) return true;
            if (auths.Contains("系统管理员")) return false;
            #region 确定当前用户角色是否属于指定的角色
            //获取当前用户所在角色
            string[] userRoles = GetUserAuthority(userid, db);
            if (userRoles == null) return false;
            //验证是否属于对应角色
            foreach (string auth in auths)
            {
                if (userRoles.Contains(auth))
                {
                    return true;
                }
            }
            #endregion
            return false;
        }
        static string[] GetUserAuthority(int userId, FundsContext db)
        {
            string[] userRoles;
            string cache_key = "user_vs_roles-" + userId;
            object objUVR = DataCache.GetCache(cache_key);
            if (objUVR == null)
            {
                userRoles = (from user in db.User_Info
                             join uvr in db.User_vs_Role on user.user_id equals uvr.uvr_user_id
                             join rva in db.Role_vs_Authority on uvr.uvr_role_id equals rva.rva_role_id
                             join auth in db.Sys_Authority on rva.rva_auth_id equals auth.auth_id
                             where user.user_id == userId
                             select auth.auth_name
                                 ).ToArray();
                if (userRoles.Count() == 0) return null;
                DataCache.SetCache(cache_key, userRoles);
            }
            else userRoles = (string[])objUVR;
            return userRoles;
        }
        static bool CheckIsSuperAdmin(int userId, FundsContext db)
        {
            var query = from uvr in db.User_vs_Role
                        where uvr.uvr_user_id == userId && uvr.uvr_role_id == 1
                        select uvr.uvr_user_id;
            if (query.Count() > 0) return true;
            else return false;
        }
    }
}