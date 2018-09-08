using System.Text.RegularExpressions;
namespace Lythen.Common
{
    public static class PasswordUnit
    {
        static Regex reg = new Regex(@"\d");
        public static string getPassword(string psw,string salt)
        {
            int first = 0;
            Match m = reg.Match(salt);
            if (m != null) first = PageValidate.FilterParam(m.Value);
            int len1 = psw.Length;
            int len2 = salt.Length;
            int len3 = len1 + len2;
            char[] newpasw = new char[len3];
            int l1=0,l2=0;
            for(int i = 0; i < len3; i++)
            {
                if (i < first)
                {
                    newpasw[i] = psw[i];
                    l1++;
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        newpasw[i] = psw[l1];
                        l1++;
                    }
                    else
                    {
                        if (l2 >= len2)
                        {
                            newpasw[i] = psw[l1];
                            l1++;
                        }
                        else
                        {
                            newpasw[i] = salt[l2];
                            l2++;
                        }
                    }

                }
            }
            return new string(newpasw);
        }
    }
}