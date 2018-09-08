using Lythen.Common;
using Lythen.DAL;
using Lythen.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Text;

namespace Lythen.Controllers
{
    public static class SysLog
    {
        public static void WriteLog(int user_id,string info,string ip,string target,int type,string device, FundsContext db)
        {
            Sys_Log log = new Sys_Log
            {
                log_content = info,
                log_device = device,
                log_ip = ip,
                log_target = target,
                log_time = DateTime.Now,
                log_type = type,
                log_user_id = user_id
            };
            db.Sys_Log.Add(log);
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder errors = new StringBuilder();
                IEnumerable<DbEntityValidationResult> validationResult = ex.EntityValidationErrors;
                foreach (DbEntityValidationResult result in validationResult)
                {
                    ICollection<DbValidationError> validationError = result.ValidationErrors;
                    foreach (DbValidationError err in validationError)
                    {
                        errors.Append(err.PropertyName + ":" + err.ErrorMessage + "\r\n");
                    }
                }
                ErrorUnit.WriteErrorLog(errors.ToString(), "WriteLog");
            }
            catch (Exception e) { ErrorUnit.WriteErrorLog(e.ToString(),"WriteLog"); }
        }
    }
}