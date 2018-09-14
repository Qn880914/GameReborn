using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Utility
{
    public static class StringExtent
    {
        /// <summary>
        /// Capitalizes only the first letter of a string
        /// </summary>
        /// <returns>string</returns>
        public static string Capitalize(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}
