using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.TestingGUI
{
    static class StringHelper
    {
        private const string NULL_VALUE = "<unavailable>";
        private const string EMPTY_VALUE = "<empty>";

        public static string ToJointString<T>(this IEnumerable<T> source)
        {
            return source.ToJointString(e => e.ToString());
        }

        public static string ToJointString<T>(this IEnumerable<T> source,
            string nullValue, string emptyValue)
        {
            return source.ToJointString(e => e.ToString(), nullValue, emptyValue);
        }

        public static string ToJointString<T>(this IEnumerable<T> source,
            Func<T, string> elementToString,
            string nullValue = NULL_VALUE,
            string emptyValue = EMPTY_VALUE)
        {
            if (source == null)
            {
                return nullValue;
            }
            else if (!source.Any())
            {
                return emptyValue;
            }
            else
            {
                return string.Join(Environment.NewLine, source.Select(elementToString));
            }
        }

        public static Task<string> ToJointStringAsync<T>(this IEnumerable<T> source)
        {
            return Task.Run(() => source.ToJointString());
        }

        public static Task<string> ToJointStringAsync<T>(this IEnumerable<T> source,
            string nullValue, string emptyValue)
        {
            return Task.Run(() => source.ToJointString(nullValue, emptyValue));
        }

        public static Task<string> ToJointStringAsync<T>(this IEnumerable<T> source, Func<T, string> elementToString)
        {
            return Task.Run(() => source.ToJointString(elementToString));
        }

        public static Task<string> ToJointStringAsync<T>(this IEnumerable<T> source, 
            Func<T, string> elementToString,
            string nullValue, string emptyValue)
        {
            return Task.Run(() => source.ToJointString(elementToString, nullValue, emptyValue));
        }
    }
}
