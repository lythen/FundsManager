using Lythen.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lythen.DAL
{
    public class Statistics
    {
        private FundsContext db;
        public Statistics(FundsContext _db)
        {
            if (_db == null) db = new FundsContext();
            else db = _db;
        }
        /// <summary>
        /// 统计
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public List<FundsStat> GetFundsStatistics(StatisticsSearch info)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select f_id as id,f_name as name,f_amount as amount,f_balance as balance");
            sql.Append(",isnull((select sum(r_fact_amount) from Reimbursements where r_funds_id=f_id and r_bill_state=1),0) as hasUsed");
            sql.Append(",(select count(1) from Reimbursements where r_funds_id=f_id and r_bill_state=1) as applyNum");
            sql.Append(" from Funds where 1=1");
            if (info.fund > 0) sql.Append(" and f_id=").Append(info.fund);
            if (info.manager > 0) sql.Append(" and f_manager=").Append(info.manager);
            sql.Append("   order by f_id");
            return db.Database.SqlQuery<FundsStat>(sql.ToString()).ToList();
        }
        /// <summary>
        /// 区间查询
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public List<FundsStat> GetTimesStatistics(StatisticsSearch info)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select isnull(sum(r_bill_amount),0) as hasUsed,r_funds_id as id,COUNT(r_funds_id) as applyNum,f_name as name");
            sql.Append(" from Reimbursements");
            sql.Append(" inner join Funds on f_id=r_funds_id");
            sql.Append(" where r_bill_state=1");
            if (info.fund > 0) sql.Append(" and f_id=").Append(info.fund);
            if (info.manager > 0) sql.Append(" and f_manager=").Append(info.manager);
            if (info.userId > 0) sql.Append(" and r_add_user_id=").Append(info.userId);
            if (info.beginDate != null) sql.Append(" and r_add_date>='").Append(((DateTime)info.beginDate).ToString("yyyy-MM-dd 00:00:00")).Append("'");
            if (info.endDate != null) sql.Append(" and r_add_date<='").Append(((DateTime)info.endDate).ToString("yyyy-MM-dd 23:59:59.999")).Append("'");
            sql.Append(" group by r_funds_id,f_name order by r_funds_id");
            return db.Database.SqlQuery<FundsStat>(sql.ToString()).ToList();
        }
    }
}