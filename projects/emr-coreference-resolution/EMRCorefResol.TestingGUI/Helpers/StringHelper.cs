using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.TestingGUI
{
    static class StringHelper
    {
        public static string ToJointString<T>(this IEnumerable<T> source)
        {
            return source.ToJointString(e => e.ToString());
        }

        public static string ToJointString<T>(this IEnumerable<T> source, Func<T, string> elementToString)
        {
            return string.Join(Environment.NewLine, source.Select(elementToString));
        }

        public static Task<string> ToJointStringAsync<T>(this IEnumerable<T> source)
        {
            return Task.Run(() => source.ToJointString());
        }

        public static Task<string> ToJointStringAsync<T>(this IEnumerable<T> source, Func<T, string> elementToString)
        {
            return Task.Run(() => source.ToJointString(elementToString));
        }
    }
}
