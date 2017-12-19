using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Text;

namespace FundsManager.Common
{
    public static class ErrorUnit
    {
        static string log_path = ConfigurationManager.AppSettings["logPath"];
        public static void WriteErrorLog(string errTxt,string where)
        {
            if (!Directory.Exists(log_path))
                Directory.CreateDirectory(log_path);
            string file_name = string.Format("{0}error_{1}.txt", log_path, DateTime.Now.ToString("yyyyMMdd"));
            using(StreamWriter sw = new StreamWriter(file_name, true, Encoding.UTF8))
            {
                sw.WriteLine(where);
                sw.WriteLine(errTxt);
                sw.WriteLine();
            }
        }
    }
}