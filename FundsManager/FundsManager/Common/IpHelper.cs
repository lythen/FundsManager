using System.Net;
using System.Web;

namespace FundsManager.Common
{
    public static class IpHelper
    {
        public static string GetIP()
        {
            string ip = "";
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null) // 服务器， using proxy
            {

                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();  // Return real client IP.

            }

            else//如果没有使用代理服务器或者得不到客户端的ip  not using proxy or can't get the Client IP
            {             //得到服务端的地址    

                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(); //While it can't get the Client IP, it will return proxy IP.

            }
            if (ip == "::1" || string.IsNullOrEmpty(ip))
            {
                string HostName = Dns.GetHostName();
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    if (IpEntry.AddressList[i].AddressFamily.ToString() == "InterNetwork")
                    {
                        ip = IpEntry.AddressList[i].ToString();
                        break;
                    }
                }
            }
            return ip;
        }
    }
}