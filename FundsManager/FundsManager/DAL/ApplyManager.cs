using FundsManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace FundsManager.DAL
{
    public class ApplyManager
    {
        private FundsContext db;
        public ApplyManager(FundsContext _db) {
            db = _db;
        }
        /// <summary>
        /// 获取主报帐单列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<ApplyListModel> GetApplyList(BillsSearchModel search)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select r_add_user_id as userId,r_bill_amount as amount,reimbursement_code as reimbursementCode,r_bill_state as state,drs_state_name as strState,r_add_date as time,f_code as  fundsCode,f_name as fundsName,reimbursement_info as info,");
            sql.Append("(select COUNT(attachment_id) from Reimbursement_Attachment where atta_reimbursement_code=reimbursement_code) as attachmentsCount");
            sql.Append(" from Reimbursements");
            sql.Append(" inner join Dic_Respond_State on drs_state_id=r_bill_state");
            sql.Append(" inner join Funds on f_id=r_funds_id");
            sql.Append(" where 1=1");
            if (search.fid != 0) sql.Append(" and r_funds_id=").Append(search.fid);
            if (search.userId != 0) sql.Append(" and r_add_user_id=").Append(search.userId);
            if (search.state != -1) sql.Append(" and r_bill_state=").Append(search.state);
            if (!string.IsNullOrEmpty(search.reimbursementCode)) sql.Append(" and reimbursement_code='").Append(search.reimbursementCode).Append("'");
            if (search.beginDate != null) sql.Append(" and r_add_date>'").Append(((DateTime)search.beginDate).ToString()).Append("'");
            if (search.endDate != null) sql.Append(" and r_add_date<'").Append(((DateTime)search.endDate).ToString()).Append("'");
            if (!string.IsNullOrEmpty(search.KeyWord)) sql.Append(" and reimbursement_info like '%").Append(search.KeyWord).Append("%'");
            if (search.PageSize > 0)
                return db.Database.SqlQuery<ApplyListModel>(sql.ToString()).Skip(search.PageSize * (search.PageIndex - 1)).Take(search.PageSize).ToList();
            else return db.Database.SqlQuery<ApplyListModel>(sql.ToString()).ToList();
        }
        /// <summary>
        /// 获取报帐单详细
        /// </summary>
        /// <param name="reimbursement_code"></param>
        /// <returns></returns>
        public IQueryable<ApplyListModel> GetReimbursement(string reimbursement_code,int userId)
        {
            var query = from bill in db.Reimbursement
                        join das in db.Dic_Respond_State
                            on bill.r_bill_state equals das.drs_state_id
                        join f in db.Funds on bill.r_funds_id equals f.f_id
                        join u in db.User_Info on bill.r_add_user_id equals u.user_id
                        select new ApplyListModel
                        {
                            amount = bill.r_bill_amount,
                            reimbursementCode = bill.reimbursement_code,
                            state = bill.r_bill_state,
                            strState = das.drs_state_name,
                            time = bill.r_add_date,
                            fundsCode = f.f_code,
                            fundsName = f.f_name,
                            userName = u.real_name,
                            info = bill.reimbursement_info,
                             userId=bill.r_add_user_id
                        };
            if (!string.IsNullOrEmpty(reimbursement_code)) query = query.Where(x => x.reimbursementCode == reimbursement_code);
            if (userId > 0)
                query = query.Where(x => x.userId == userId);
            return query;
        }
        /// <summary>
        /// 获取批复查询
        /// </summary>
        /// <param name="reimbursement_code"></param>
        /// <param name="respondId"></param>
        /// <returns></returns>
        public IQueryable<Respond> getResponds(string reimbursement_code,int respondId)
        {
            var query = from respond in db.Process_Respond
                        join state in db.Dic_Respond_State on respond.pr_state equals state.drs_state_id
                        join user in db.User_Info on respond.pr_user_id equals user.user_id
                        where respond.pr_reimbursement_code== reimbursement_code
                        select new Respond
                        {
                            id = respond.pr_id,
                            state = respond.pr_state,
                            next = respond.next,
                            num = respond.pr_number,
                            reason = respond.pr_content,
                            respondDate = respond.pr_time,
                            respondUser = user.real_name,
                            respondUserId = user.user_id,
                            strState = state.drs_state_name
                        };
            if (respondId > 0)
                query = query.Where(x => x.id == respondId);
            return query;
        }
        /// <summary>
        /// 获取报销内容明细查询
        /// </summary>
        /// <param name="contendId"></param>
        /// <param name="detailId"></param>
        /// <returns></returns>
        public IQueryable<ViewDetailContent> getContentDetails(int contendId,int detailId)
        {
            var query = from detail in db.Reimbursement_Detail
                        where detail.detail_content_id == contendId
                        select new ViewDetailContent
                        {
                            amount = detail.detail_amount,
                            contentId = detail.detail_content_id,
                            detailDate = detail.detail_date,
                            detailId = detail.detail_id,
                            detailInfo = detail.detail_info
                        };
            if (detailId > 0)
                query = query.Where(x => x.detailId == detailId);
            return query;
        }
        /// <summary>
        /// 获取报销内容查询
        /// </summary>
        /// <param name="reimbursement_code"></param>
        /// <param name="contendId"></param>
        /// <returns></returns>
        public IQueryable<ViewContentModel> getContents(string reimbursement_code,int contendId)
        {
            var query = from content in db.Reimbursement_Content
                        join dic in db.Dic_Reimbursement_Content on content.c_dic_id equals dic.content_id
                        where content.c_reimbursement_code == reimbursement_code
                        select new ViewContentModel
                        {
                            amount = content.c_amount,
                            contentId = content.content_id,
                            contentTitle = dic.content_title,
                            reimbursementCode = content.c_reimbursement_code,
                            selectId = content.c_dic_id
                        };
            if (contendId > 0)
                query = query.Where(x => x.contentId == contendId);
            return query;
        }
        /// <summary>
        /// 获取附件列表查询
        /// </summary>
        /// <param name="reimbursement_code"></param>
        /// <param name="atta_id"></param>
        /// <returns></returns>
        public IQueryable<ViewAttachment> getAttachments(string reimbursement_code,int atta_id)
        {
            var query = from attachment in db.Reimbursement_Attachment
                        where attachment.atta_reimbursement_code == reimbursement_code
                        select new ViewAttachment
                        {
                            fileName = attachment.attachment_path,
                            id = attachment.attachment_id,
                            reimbursementCode = attachment.atta_reimbursement_code
                        };
            if (atta_id > 0)
                query = query.Where(x => x.id == atta_id);
            return query;
        }
    }
}