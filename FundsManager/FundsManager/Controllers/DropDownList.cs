using System.Collections.Generic;
using System.Web.Mvc;
using FundsManager.DAL;
using FundsManager.Models;
using System.Linq;
using FundsManager.Common;
using FundsManager.ViewModels;
using FundsManager.Common.DEncrypt;

namespace FundsManager.Controllers
{
    public static class DropDownList
    {
        private static FundsContext db = new FundsContext();
        private static string cache_week = "cache_week";
        private static string cache_post = "cache_post";
        private static string cache_funds_manger = "funds_manger";
        private static string cache_sex = "cache_sex";
        private static string cache_user_state = "cache_user_state";
        private static string cache_role = "cache_role";
        private static string cache_cardType = "cache_cardType";
        private static string cache_funds = "cache_funds_";
        private static string cache_process = "cache_process";
        private static string cache_stat_detail = "cache_stat_detail";
        private static string cache_response_user = "cache_response_user_";
        private static string cache_authority = "cache_authority";
        private static string cache_content = "cache_content";
        private static string cache_user = "cache_user";
        public static List<SelectListItem> SetDropDownList(List<Models.SelectOption> options)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (options != null)
            {
                foreach (Models.SelectOption item in options)
                {
                    items.Add(new SelectListItem { Text = item.text, Value = item.id });
                }
            }
            return items;
        }
        public static List<SelectOption> WeekSelect()
        {
            List<SelectOption> options = (List<SelectOption>)DataCache.GetCache(cache_week);
            if (options == null)
            {
                options = new List<SelectOption>();
                options.Add(new SelectOption { text = "日", id = "0" });
                options.Add(new SelectOption { text = "一", id = "1" });
                options.Add(new SelectOption { text = "二", id = "2" });
                options.Add(new SelectOption { text = "三", id = "3" });
                options.Add(new SelectOption { text = "四", id = "4" });
                options.Add(new SelectOption { text = "五", id = "5" });
                options.Add(new SelectOption { text = "六", id = "6" });
                DataCache.SetCache(cache_week, options);
            }
            return options;
        }
        public static List<SelectOption> PostSelect()
        {
            List<SelectOption> options = new List<SelectOption>();
            var post = DBCaches<Dic_Post>.getCache(cache_post);
            if (post != null)
            {
                foreach (var item in post)
                {
                    options.Add(new SelectOption { text = item.post_name, id = item.post_id.ToString() });
                }
            }
            return options;
        }
        public static List<SelectOption> FundsManagerSelect(int userId)
        {
            string key;
            bool isManager = RoleCheck.CheckHasAuthority(userId, db, "经费管理");
            if (isManager) key = cache_response_user;
            else key = cache_response_user + userId;
            List<SelectOption> options = (List<SelectOption>)DataCache.GetCache(key);
            if (options == null)
            {
                var query = (from funds in db.Funds
                             join user in db.User_Info on funds.f_manager equals user.user_id
                             group funds by new {funds.f_manager,user.user_name} into g
                             select new
                             {
                                 userId = g.Key.f_manager,
                                 userName=g.Key.user_name
                             }
                    ).ToList();
                if (userId > 0 && !isManager)
                    query = query.Where(x => x.userId == userId).ToList();
                options = (from user in query
                           select new SelectOption
                           {
                               id = user.userId.ToString(),
                               text = AESEncrypt.Decrypt(user.userName)
                           }).ToList();
                if (isManager) options.Insert(0,new SelectOption { id = "0", text = "全部" });
                if (options.Count() > 0) DataCache.SetCache(key, options);
            }
            return options;
        }
        public static List<SelectOption> getDepartment(int? id = null)
        {
            List<DepartMentModel> depts = DBCaches2.getDeptCache();
            List<SelectOption> option = (from dep in depts
                                         where dep.parentId == (id == null ? dep.parentId : (int)id)
                                         select new SelectOption
                                         {
                                             id = dep.deptId.ToString(),
                                             text = dep.deptName
                                         }).ToList();
            return option;
        }
        public static List<SelectOption> SexSelect()
        {
            List<SelectOption> options = (List<SelectOption>)DataCache.GetCache(cache_sex);
            if (options == null)
            {
                options = new List<SelectOption>();
                options.Add(new SelectOption { text = "男", id = "男" });
                options.Add(new SelectOption { text = "女", id = "女" });
                DataCache.SetCache(cache_sex, options);
            }
            return options;
        }
        public static List<SelectOption> UserSelect(int userId)
        {
            string key = cache_user + userId;
            bool isManager = RoleCheck.CheckHasAuthority(userId, db, "经费管理");
            if (isManager) key = cache_user;
            else key = cache_user + userId;
            List<SelectOption> options = (List<SelectOption>)DataCache.GetCache(key);
            if (options == null)
            {
                var query = (from user in db.User_Info
                            where user.user_state == 1
                            select new
                            {
                                userId=user.user_id,
                                userName= user.real_name
                            }).ToList();
                if (userId > 0 && !isManager)
                    query = query.Where(x => x.userId == userId).ToList();

                options = (from user in query
                           select new SelectOption
                           {
                               id = user.userId.ToString(),
                               text = AESEncrypt.Decrypt(user.userName)
                           }).ToList();
                if (isManager) options.Insert(0,new SelectOption { id = "0", text = "全部" });
                if (options.Count() > 0) DataCache.SetCache(key, options);
            }
            return options;
        }
        public static List<SelectOption> UserStateSelect()
        {
            List<SelectOption> options = (List<SelectOption>)DataCache.GetCache(cache_user_state);
            if (options == null)
            {
                options = new List<SelectOption>();
                options.Add(new SelectOption { text = "正常", id = "1" });
                options.Add(new SelectOption { text = "未启用", id = "0" });
                options.Add(new SelectOption { text = "锁定", id = "2" });
                DataCache.SetCache(cache_user_state, options);
            }
            return options;
        }
        public static List<SelectOption> CardTypeSelect()
        {
            List<Dic_CardType> depts = DBCaches<Dic_CardType>.getCache(cache_cardType);
            List<SelectOption> option = (from ct in depts
                                         select new SelectOption
                                         {
                                             id = ct.ctype_name,
                                             text = ct.ctype_name
                                         }).ToList();
            return option;
        }
        public static List<SelectOption> RoleSelect(string ignor)
        {
            List<Dic_Role> depts = DBCaches<Dic_Role>.getCache(cache_role);
            List<SelectOption> option = (from ct in depts
                                         where ct.role_name!=ignor
                                         select new SelectOption
                                         {
                                             id = ct.role_id.ToString(),
                                             text = ct.role_name
                                         }).ToList();
            return option;
        }
        public static List<SelectOption> FundsSelect(int user)
        {
            string key = cache_funds+user;
            List<Funds> funds = DBCaches<Funds>.getCache(cache_funds);
            List<SelectOption> options = (List<SelectOption>)DataCache.GetCache(key);
            if (options == null)
            {
                var query = from fund in funds
                            where fund.f_state == 1
                            select fund;
                if(user>0&&!RoleCheck.CheckHasAuthority(user,db, "经费管理"))
                    query = query.Where(x => x.f_manager == user);
                options = (from fund in query
                           select new SelectOption
                                             {
                                                 id = fund.f_id.ToString(),
                                                 text = string.Format("{0}({1})", fund.f_name, fund.f_code)
                                             }).ToList();
                if (options.Count() > 0) DataCache.SetCache(key, options);
            }
            return options;
        }
        public static List<SelectOption> ProcessSelect()
        {
            List<Process_Info> funds = DBCaches<Process_Info>.getCache(cache_process);
            List<SelectOption> option = (from fund in funds
                                         select new SelectOption
                                         {
                                             id = fund.process_id.ToString(),
                                             text = fund.process_name
                                         }).ToList();
            return option;
        }
        public static List<SelectOption> RespondUserSelect()
        {
            string key = cache_response_user + "0";
            List<SelectOption> options = (List<SelectOption>)DataCache.GetCache(key);
            if (options == null)
            {
                options = (from op in (from user in db.User_Info
                                       join uvr in db.User_vs_Role on user.user_id equals uvr.uvr_user_id
                                       where user.user_state == 1 && uvr.uvr_role_id <=2
                                       select new
                                       {
                                           id = user.user_id,
                                           text = user.real_name
                                       }).ToList()
                           select new SelectOption
                           {
                               id = op.id.ToString(),
                               text = Common.DEncrypt.AESEncrypt.Decrypt(op.text)
                           }).ToList();
                if (options.Count() > 0) DataCache.SetCache(key, options);
            }
            return options;
        }
        public static List<AuthInfo> AuthoritySelect()
        {
            List<Sys_Authority> funds = DBCaches<Sys_Authority>.getCache(cache_authority);
            List<AuthInfo> option = (from auth in funds
                                         select new AuthInfo
                                         {
                                             authId = auth.auth_id,
                                             authInfo = auth.auth_info,
                                             authName = auth.auth_name,
                                             isController = auth.auth_is_Controller,
                                             mapController = auth.auth_map_Controller
                                         }).ToList();
            return option;
        }
        public static List<SelectOption> ContentSelect()
        {
            List<Dic_Reimbursement_Content> contents = DBCaches<Dic_Reimbursement_Content>.getCache(cache_content);
            List<SelectOption> option = (from content in contents
                                         select new SelectOption
                                         {
                                             id = content.content_id.ToString(),
                                             text = content.content_title
                                         }).ToList();
            return option;
        }
    }
}