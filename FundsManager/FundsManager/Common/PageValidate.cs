using System;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace FundsManager.Common
{
    /// <summary>
    /// 页面数据校验类
    /// </summary>
    public class PageValidate
    {
        private static Regex RegNumber = new Regex("^[0-9]+$");
        private static Regex RegNumberSign = new Regex("^[+-]?[0-9]+$");
        private static Regex RegDecimal = new Regex("^[0-9]+[.]?[0-9]+$");
        private static Regex RegDecimalSign = new Regex("^[+-]?[0-9]+[.]?[0-9]+$"); //等价于^[+-]?\d+[.]?\d+$
        private static Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$");//w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样 
        private static Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");
        private static Regex RegTaiWan = new Regex(@"^[A-Za-z0-9\(\)]+$");
        public PageValidate()
        {
        }


        #region 数字字符串检查

        /// <summary>
        /// 检查Request查询字符串的键值，是否是数字，最大长度限制
        /// </summary>
        /// <param name="req">Request</param>
        /// <param name="inputKey">Request的键值</param>
        /// <param name="maxLen">最大长度</param>
        /// <returns>返回Request查询字符串</returns>
        public static string FetchInputDigit(HttpRequest req, string inputKey, int maxLen)
        {
            string retVal = string.Empty;
            if (inputKey != null && inputKey != string.Empty)
            {
                retVal = req.QueryString[inputKey];
                if (null == retVal)
                    retVal = req.Form[inputKey];
                if (null != retVal)
                {
                    retVal = SqlText(retVal, maxLen);
                    if (!IsNumber(retVal))
                        retVal = string.Empty;
                }
            }
            if (retVal == null)
                retVal = string.Empty;
            return retVal;
        }
        /// <summary>
        /// 是否数字字符串
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumber(string inputData)
        {
            Match m = RegNumber.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 是否数字字符串 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumberSign(string inputData)
        {
            Match m = RegNumberSign.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 是否是浮点数
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimal(string inputData)
        {
            Match m = RegDecimal.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 是否是浮点数 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimalSign(string inputData)
        {
            Match m = RegDecimalSign.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 过滤整型的ID参数，非法的参数将返回0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int FilterParam(string s)
        {
            if (IsNumberSign(s))
                return int.Parse(s);
            else
                return 0;
        }
        /// <summary>
        /// 过滤整型的ID参数，非法的参数将返回0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Int32 FilterParamInt32(string s)
        {
            if (IsNumberSign(s))
                return Int32.Parse(s);
            else
                return 0;
        }
        /// <summary>
        /// 过滤整型的ID参数，非法的参数将返回0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long FilterLongParam(string s)
        {
            if (IsNumberSign(s))
                return long.Parse(s);
            else
                return 0;
        }
        #endregion

        #region 中文检测

        /// <summary>
        /// 检测是否有中文字符
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsHasCHZN(string inputData)
        {
            Match m = RegCHZN.Match(inputData);
            return m.Success;
        }

        #endregion

        #region 邮件地址
        /// <summary>
        /// 是否是浮点数 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsEmail(string inputData)
        {
            Match m = RegEmail.Match(inputData);
            return m.Success;
        }

        #endregion

        #region 二代身份证号码

        /// <summary>
        /// 验证身份证号码（适应15位或18位）
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool CheckIDCardNo(string Id)
        {
            if (Id.Length == 18)
            {
                bool check = CheckIDCard18(Id);
                return check;
            }
            else if (Id.Length == 15)
            {
                bool check = CheckIDCard15(Id);
                return check;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 验证港澳往来居住证号码
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool CheckMHIDCardNo(string Id)
        {
            if (!Id.ToUpper().StartsWith("M") && !Id.ToUpper().StartsWith("H"))
                return false;
            if (Id.Length != 9 && Id.Length != 11)
            {
                return false;
            }
            return true;
        }
        public static bool CheckTWIDCardNo(string Id)
        {
            Match m = RegTaiWan.Match(Id);
            return m.Success;
        }
        /// <summary>
        /// 验证18位身份证格式
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// 验证15位身份证格式
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }


        /// <summary>
        /// 根据身份证号获取生日
        /// </summary>
        /// <param name="IdCard"></param>
        /// <returns></returns>
        public static string GetBrithdayFromIdCard(string IdCard)
        {
            string rtn = "1900-01-01";
            if (IdCard.Length == 15)
            {
                rtn = IdCard.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            }
            else if (IdCard.Length == 18)
            {
                rtn = IdCard.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            }
            return rtn;
        }


        /// <summary>
        /// 根据身份证获取性别
        /// </summary>
        /// <param name="IdCard"></param>
        /// <returns></returns>
        public static string GetSexFromIdCard(string IdCard)
        {
            string rtn;
            string tmp = "";
            if (IdCard.Length == 15)
            {
                tmp = IdCard.Substring(IdCard.Length - 3);
            }
            else if (IdCard.Length == 18)
            {
                tmp = IdCard.Substring(IdCard.Length - 4);
                tmp = tmp.Substring(0, 3);
            }
            int sx = int.Parse(tmp);
            int outNum;
            Math.DivRem(sx, 2, out outNum);
            if (outNum == 0)
            {
                rtn = "女";
            }
            else
            {
                rtn = "男";
            }
            return rtn;
        }

        #endregion

        #region 是否是时间格式
        /// <summary>
        /// 判断一个字符串是否时间格式
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsDateTime(string inputData)
        {
            try
            {
                Convert.ToDateTime(inputData);
                return true;//是   
            }
            catch
            {
                return false;//不是   
            }
        }

        #endregion

        #region 其他

        /// <summary>
        /// 检查字符串最大长度，返回指定长度的串
        /// </summary>
        /// <param name="sqlInput">输入字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns></returns>			
        public static string SqlText(string sqlInput, int maxLength)
        {
            if (sqlInput != null && sqlInput != string.Empty)
            {
                sqlInput = sqlInput.Trim();
                if (sqlInput.Length > maxLength)//按最大长度截取字符串
                    sqlInput = sqlInput.Substring(0, maxLength);
            }
            return sqlInput;
        }
        /// <summary>
        /// 字符串编码
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static string HtmlEncode(string inputData)
        {
            return HttpUtility.HtmlEncode(inputData);
        }
        /// <summary>
        /// 设置Label显示Encode的字符串
        /// </summary>
        /// <param name="lbl"></param>
        /// <param name="txtInput"></param>
        public static void SetLabel(Label lbl, string txtInput)
        {
            lbl.Text = HtmlEncode(txtInput);
        }
        public static void SetLabel(Label lbl, object inputObj)
        {
            SetLabel(lbl, inputObj.ToString());
        }
        //字符串清理
        public static string InputText(string inputString, int maxLength)
        {
            StringBuilder retVal = new StringBuilder();

            // 检查是否为空
            if ((inputString != null) && (inputString != String.Empty))
            {
                inputString = inputString.Trim();

                //检查长度
                if (inputString.Length > maxLength)
                    inputString = inputString.Substring(0, maxLength);

                //替换危险字符
                for (int i = 0; i < inputString.Length; i++)
                {
                    switch (inputString[i])
                    {
                        case '"':
                            retVal.Append("&quot;");
                            break;
                        case '<':
                            retVal.Append("&lt;");
                            break;
                        case '>':
                            retVal.Append("&gt;");
                            break;
                        default:
                            retVal.Append(inputString[i]);
                            break;
                    }
                }
                retVal.Replace("'", " ");// 替换单引号
            }
            return retVal.ToString();

        }
        /// <summary>
        /// 转换成 HTML code
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>string</returns>
        public static string Encode(string str)
        {
            if (String.IsNullOrEmpty(str)) return str;
            str = str.Replace("&", "&amp;");
            str = str.Replace("'", "''");
            str = str.Replace("\"", "&quot;");
            str = str.Replace(" ", "&nbsp;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("\n", "<br>");
            return str;
        }
        /// <summary>
        ///解析html成 普通文本
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>string</returns>
        public static string Decode(string str)
        {
            if (String.IsNullOrEmpty(str)) return str;
            str = str.Replace("<br>", "\n");
            str = str.Replace("&gt;", ">");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&nbsp;", " ");
            str = str.Replace("&quot;", "\"");
            return str;
        }

        public static DateTime? FormateDateTime(string strTime)
        {
            DateTime? dt = null;
            int strlen = strTime.Length;
            try
            {
                if (strTime.Contains("-"))
                {
                    string[] timeList = strTime.Split(new char[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (timeList.Length >= 3)
                    {
                        string y = timeList[0];
                        string m = timeList[1];
                        string d = timeList[2];
                        if (y.Length == 2)
                        {
                            if (Int32.Parse(y) < 50)
                                y = "20" + y;
                            else y = "19" + y;
                        }
                        if (m.Length == 1) m = "0" + m;
                        if (d.Length == 1) d = "0" + d;
                        if (timeList.Length == 4)
                            dt = DateTime.Parse(string.Format("{0}-{1}-{2} {3}", y, m, d, timeList[3]));
                        else
                            dt = DateTime.Parse(string.Format("{0}-{1}-{2} 00:00:00.000", y, m, d));
                    }
                }
                else if (strlen == 6)
                {
                    if (strTime.Substring(0, 2) != "19" && strTime.Substring(0, 2) != "20")
                        dt = DateTime.ParseExact(strTime, "yyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    else if (Int32.Parse(strTime.Substring(strlen - 2, 2)) > 12)
                    {
                        string subStr = string.Format("{0}0{1}0{2}", strTime.Substring(0, 4), strTime.Substring(4, 1), strTime.Substring(5, 1));
                        dt = DateTime.ParseExact(subStr, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    else dt = DateTime.ParseExact(strTime, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);
                }
                else if (strlen == 8) dt = DateTime.ParseExact(strTime, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                else dt = DateTime.Parse(strTime);
            }
            catch (FormatException e)
            {
                return null;
            }
            return dt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceHtmlStr(String str)
        {
            str = Regex.Replace(str, @"&nbsp;", " ", RegexOptions.IgnoreCase).Trim();
            str = Regex.Replace(str, @"<.+?>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"&(quot|#34);", "＼", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"&(amp|#38);", "＆", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"&(lt|#60);", "＜", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"&(gt|#62);", "＞", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            str = str.Replace("'", "''");
            return str;
        }

        #endregion

    }
}
