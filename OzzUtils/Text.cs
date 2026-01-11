using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace OzzUtils
{
    public static partial class Text
    {
        public static bool IsNumeric(this string s)
        {
            return Regex.IsMatch(s, @"^\d+$");
        }

        public static string ToTitleCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            var textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
            return  textInfo.ToTitleCase(s);
        }

        public static string ToSentenceCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            var lowerCase = s.ToLower();
            var r = new Regex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture);
            return r.Replace(lowerCase, l => l.Value.ToUpper());
        }

        public static string PascalCaseToTitleCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return Regex.Replace(s, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToUpperInvariant(m.Value[1]));
        }

        public static string PascalCaseToSentenceCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return Regex.Replace(s, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLowerInvariant(m.Value[1]));
        }

        /// <summary>
        /// Returns new string which spaces are removed and each word begins with a capital letter
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string s)
        {

            if (string.IsNullOrEmpty(s))
                return string.Empty;
            
            if (s.Length == 1)
                return s[0].ToString().ToUpperInvariant();
            
            string[] words = s.Split(' ');
            if (words.Length == 1)
                return s.ToTitleCase()[0].ToString().ToUpperInvariant() + s.Substring(1);
            
            StringBuilder sb = new StringBuilder();
            foreach (var word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    sb.Append(word[0].ToString().ToUpperInvariant());
                    sb.Append(word.Substring(1).ToLowerInvariant());
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns new string with the first letter changed to lowercase
        /// </summary>
        public static string ToCamelCase(this string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;

            if (s.Length == 1)
                return s[0].ToString().ToLowerInvariant();

            return s[0].ToString().ToLowerInvariant() + s.ToPascalCase().Substring(1);
        }

        public static string StripNamespace(this string s)
        {
            string[] parts = s.Split('.');
            return parts.LastOrDefault();
        }

        public static string GetNamespace(this string s)
        {
            string[] parts = s.Split('.');
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (sb.Length > 0)
                {
                    sb.Append('.');
                }
                sb.Append(parts[i]);
            }

            return sb.ToString();
        }


        public static string Pluralize(this string s)
        {
            if (s.EndsWith("y") &&
                !s.EndsWith("ay") && !s.EndsWith("ey") &&
                !s.EndsWith("uy") && !s.EndsWith("iy") && !s.EndsWith("oy"))
            {
                return s.Substring(0, s.Length - 1) + "ies";
            }
            else if (s.EndsWith("s") || s.EndsWith("h") || s.EndsWith("x"))
            {
                return s + "es";
            }
            else
            {
                return s + "s";
            }
        }

        public static string MakeVirtualPath(this string source)
        {
            return source.Trim()
                            .RemoveHtmlTags()
                            .RemoveIllegalChars()
                            .RemoveTurkishChars()
                            .Trim()
                            .Replace(' ', '-')
                            .ToLowerInvariant();
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        public static string RemoveHtmlTags(this string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }


        private static List<char[]>  ReplaceTrEnList
        {
            set { List<char[]> replaceList = value; }
            get
            {
                if (_replaceTrEnList == null)
                    SetReplaceList();
                return _replaceTrEnList;
            }
        }
        private static void SetReplaceList()
        {
            _replaceTrEnList = new List<char[]>();
            _replaceTrEnList.Add(new char[] { 'ş', 's' });
            _replaceTrEnList.Add(new char[] { 'ğ', 'g' });
            _replaceTrEnList.Add(new char[] { 'ü', 'u' });
            _replaceTrEnList.Add(new char[] { 'ö', 'o' });
            _replaceTrEnList.Add(new char[] { 'ç', 'c' });
            _replaceTrEnList.Add(new char[] { 'ı', 'i' });
            _replaceTrEnList.Add(new char[] { 'İ', 'I' });
            _replaceTrEnList.Add(new char[] { 'Ş', 'S' });
            _replaceTrEnList.Add(new char[] { 'Ç', 'C' });
            _replaceTrEnList.Add(new char[] { 'Ö', 'O' });
            _replaceTrEnList.Add(new char[] { 'Ğ', 'G' });
            _replaceTrEnList.Add(new char[] { 'Ü', 'U' });
        }
        private static List<char[]> _replaceTrEnList;

        public static string RemoveTurkishChars(this string source)
        {
            StringBuilder sb = new StringBuilder(source);

            foreach (char[] c in ReplaceTrEnList)
            {
                sb.Replace(c[0], c[1]);
            }

            return sb.ToString();
        }


        public static string EncodeAccentChars(this string source)
        {
            return EncodeTurkishChars(source);
        }



        private static List<string[]> ShortCodeList
        {
            set { _shortCodeList = value; }
            get
            {
                if (_shortCodeList == null) SetShortCodeList();
                return _shortCodeList;
            }
        }
        private static void SetShortCodeList()
        {
            _shortCodeList = new List<string[]>();
            _shortCodeList.Add(new string[] { "ş", "\\s" });
            _shortCodeList.Add(new string[] { "ğ", "\\g" });
            _shortCodeList.Add(new string[] { "ü", "\\u" });
            _shortCodeList.Add(new string[] { "ö", "\\o" });
            _shortCodeList.Add(new string[] { "ç", "\\c" });
            _shortCodeList.Add(new string[] { "ı", "\\i" });
            _shortCodeList.Add(new string[] { "İ", "\\I" });
            _shortCodeList.Add(new string[] { "Ş", "\\S" });
            _shortCodeList.Add(new string[] { "Ç", "\\C" });
            _shortCodeList.Add(new string[] { "Ö", "\\O" });
            _shortCodeList.Add(new string[] { "Ğ", "\\G" });
            _shortCodeList.Add(new string[] { "Ü", "\\U" });
        }
        private static List<string[]> _shortCodeList;

        private static List<string[]> HtmlEncodeList
        {
            set { _htmlCodeList = value; }
            get
            {
                if (_htmlCodeList == null) SetHtmlEncodeList();
                return _htmlCodeList;
            }
        }
        private static void SetHtmlEncodeList()
        {
            _htmlCodeList = new List<string[]>();
            _htmlCodeList.Add(new string[] { "ş", "&#351;" });
            _htmlCodeList.Add(new string[] { "ğ", "&#287;" });
            _htmlCodeList.Add(new string[] { "ü", "&#252;" });
            _htmlCodeList.Add(new string[] { "ö", "&#246;" });
            _htmlCodeList.Add(new string[] { "ç", "&#231;" });
            _htmlCodeList.Add(new string[] { "ı", "&#305;" });
            _htmlCodeList.Add(new string[] { "İ", "&#304;" });
            _htmlCodeList.Add(new string[] { "Ş", "&#350;" });
            _htmlCodeList.Add(new string[] { "Ç", "&#199;" });
            _htmlCodeList.Add(new string[] { "Ö", "&#214;" });
            _htmlCodeList.Add(new string[] { "Ğ", "&#286;" });
            _htmlCodeList.Add(new string[] { "Ü", "&#220;" });
        }
        private static List<string[]> _htmlCodeList;

        public static string EncodeTurkishChars(this string source, bool shortCode = false)
        {
            StringBuilder sb = new StringBuilder(source);

            var encodeList = shortCode ? ShortCodeList : HtmlEncodeList;
            foreach (string[] c in encodeList)
            {
                sb.Replace(c[0], c[1]);
            }

            return sb.ToString();
        }

        public static string DecodeTurkishChars(this string source, bool shortCode = false)
        {
            StringBuilder sb = new StringBuilder(source);

            var encodeList = shortCode ? ShortCodeList : HtmlEncodeList;
            foreach (string[] c in encodeList)
            {
                sb.Replace(c[1], c[0]);
            }

            return sb.ToString();
        }

        public static string RemoveIllegalChars(this string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            char[] removeArray = { '\'', '\"', '[', ']', '{', '}', '(', ')', '"',
                                     '%', '$','.',',','!','@','#','$','%','^','&', 
                                     '*', '”', '“', '‘', '’', ';', ':', '\\', 
                                     '\r', '\n', '>', '<', '…', 
                                     '`','~','/','?','+','=','€' };

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (!removeArray.Contains(let))
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }

            return new string(array, 0, arrayIndex);
        }

        public static string StringArrayToString(string[] array)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in array)
            {
                sb.Append(s);
                sb.Append(' ');
            }

            return sb.ToString().Trim();
        }

        public static string Singularize(this string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length < 2) return s;

            if (s.EndsWith("ies"))
            {
                return s.Substring(0, s.Length - 3) + "y";
            }
            else
            {
                return s.Substring(0, s.Length - 1);
            }
        }
    }
}
