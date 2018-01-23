using FundsManager.Common;
using FundsManager.DAL;
using FundsManager.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace FundsManager.Controllers
{
    public class DBCaches2
    {
        private static FundsContext db = new FundsContext();
        private static string cache_depts = "cache_depts";
        public static List<DepartMentModel> getDeptCache()
        {
            List<DepartMentModel> list = (List<DepartMentModel>)DataCache.GetCache(cache_depts);
            if (list == null)
            {
                list = getDept();
                if(list.Count()>0) DataCache.SetCache(cache_depts, list);
            }
            return list;
        }
        static List<DepartMentModel> getDept(int? pid = null)
        {
            var query = from dept in db.Dic_Department
                       join dept2 in db.Dic_Department on dept.dept_parent_id equals dept2.dept_id into T1
                       from t1 in T1.DefaultIfEmpty()
                       orderby dept.dept_parent_id ascending
                       select new DepartMentModel
                       {
                           deptId = dept.dept_id,
                           deptName = dept.dept_name,
                           parentName = t1.dept_name == null ? "" : t1.dept_name,
                           parentId=dept.dept_parent_id
                       };
            List<DepartMentModel> list;
            if (pid != null) list = query.Where(x => x.parentId == (int)pid).ToList();
            else list = query.ToList();
            return list;
        }
        public static void ClearCache(string cache_name)
        {
            DataCache.RemoveCache(cache_name);
        }
        public static void ClearAllCache()
        {
            DataCache.RemoveAllCache();
        }
    }
}