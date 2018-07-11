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
        private FundsContext db = new FundsContext();
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
    }
}