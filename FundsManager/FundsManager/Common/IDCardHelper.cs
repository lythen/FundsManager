namespace FundsManager.Common
{
    static public class IDCardHelper
    {
        static public string GetBirthday(string identityCard)
        {
            string birthday = "";
            //处理18位的身份证号码从号码中得到生日和性别代码
            if (identityCard.Length == 18)
            {
                birthday = identityCard.Substring(6, 4) + "-" + identityCard.Substring(10, 2) + "-" + identityCard.Substring(12, 2);
            }
            //处理15位的身份证号码从号码中得到生日和性别代码
            if (identityCard.Length == 15)
            {
                birthday = "19" + identityCard.Substring(6, 2) + "-" + identityCard.Substring(8, 2) + "-" + identityCard.Substring(10, 2);
            }
            return birthday;
        }
        public static string GetSex(string identityCard)
        {
            string sex = "";
            //处理18位的身份证号码从号码中得到生日和性别代码
            if (identityCard.Length == 18)
            {
                sex = identityCard.Substring(14, 3);
            }
            //处理15位的身份证号码从号码中得到生日和性别代码
            if (identityCard.Length == 15)
            {
                sex = identityCard.Substring(12, 3);
            }
            if (int.Parse(sex) % 2 == 0)
            {
                return "女";
            }
            else
            {
                return "男";
            }
        }
    }
}
