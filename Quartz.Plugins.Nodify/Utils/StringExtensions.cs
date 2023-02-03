using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Quartz.Plugins.Nodify.Utils
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static T To<T>(this string str)
        {
            return ((object)str).To<T>();
        }

        public static IEnumerable<T> ToEnumerable<T>(this string str, char delimiter = ',')
        {
            if (str.IsNullOrWhiteSpace())
            {
                return new List<T>();
            }

            return str.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).Select(s =>s.To<T>());
        }
    }
}
