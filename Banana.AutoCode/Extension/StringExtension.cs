using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace System
{
    /// <summary> 
    /// 作者：zhangw 
    /// 时间：2017/3/30 21:16:58 
    /// CLR版本：4.0.30319.42000 
    /// 唯一标识：76ef04aa-b4bf-4b0d-925a-3a7059606f13 
    /// </summary> 
    public static class StringExtension
    {
        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="that"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string that) where T : class
        {
            if (string.IsNullOrWhiteSpace(that))
            {
                return default(T);
            }
            return new JavaScriptSerializer().Deserialize<T>(that);
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="that"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatString(this string that, params object[] args)
        {
            return string.Format(that, args);
        }

        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string that)
        {
            return string.IsNullOrWhiteSpace(that);
        }

        /// <summary>
        /// 转换成Camel命名法
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static string ToCamel(this string that)
        {
            if (that.IsNullOrWhiteSpace()) return that;
            return that[0].ToString().ToLower() + that.Substring(1);
        }

        /// <summary>
        /// 转换成Pascal命名法
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static string ToPascal(this string that)
        {
            if (that.IsNullOrWhiteSpace()) return that;

            var arr = that.Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 1)
            {
                return that[0].ToString().ToUpper() + that.Substring(1).ToLower();
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in arr)
            {
                sb.Append(item.ToPascal());
            }
            return sb.ToString();
        }


        /// <summary>
        /// 转全角(SBC case)
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static string ToSBC(this string that)
        {
            ///任意字符串
            ///全角字符串
            ///全角空格为12288，半角空格为32
            ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248

            char[] c = that.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new String(c);
        }

        /// <summary>
        /// 转半角(DBC case)
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static String ToDBC(this string that)
        {
            // /任意字符串
            // /半角字符串
            // /全角空格为12288，半角空格为32
            // /其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248

            char[] c = that.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }

        /// <summary>
        /// 去除字符串中的html字符
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static string TrimHtml(this string that)
        {
            if (string.IsNullOrWhiteSpace(that)) return "";
            return System.Text.RegularExpressions.Regex.Replace(that, @"<[^>]*>", "").Trim();
        }

    }
}
