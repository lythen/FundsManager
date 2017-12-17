using System;
using System.Text;
using System.Web.Security;

namespace FundsManager.Common
{
    public class TokenProccessor
    {
        /*
    *单例设计模式（保证类的对象在内存中只有一个）
    *1、把类的构造函数私有
    *2、自己创建一个类的对象
    *3、对外提供一个公共的方法，返回类的对象
    */
        private TokenProccessor() { }

        private static TokenProccessor instance;
        private static object locker = new object();
        /**
         * 返回类的对象
         * @return
         */
        public static TokenProccessor getInstance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null) instance = new TokenProccessor();
                }
            }
            return instance;
        }

        /**
         * 生成Token
         * Token：Nv6RRuGEVvmGjB+jimI/gw==
         * @return
         */
        public string makeToken()
        {  //checkException
           //  7346734837483  834u938493493849384  43434384
            string token = (DateTime.Now.Millisecond + new Random().Next(999999999)) + "";
            //数据指纹   128位长   16个字节  md5
            try
            {
                string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(token, "MD5");
                //base64编码--任意二进制编码明文字符   adfsdfsdfsf
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(md5));
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}