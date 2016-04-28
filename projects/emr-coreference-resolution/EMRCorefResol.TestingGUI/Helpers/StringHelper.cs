using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.TestingGUI
{
    static class StringHelper
    {
        public static string ToStringLines<T>(this IEnumerable<T> sequence)
        {
            return sequence.ToStringLines(e => e.ToString());
        }

        public static string ToStringLines<T>(this IEnumerable<T> sequence, Func<T, string> toString)
        {
            return string.Join(Environment.NewLine, sequence.Select(e => toString(e)));
        }

        public static Task<string> ToStringLinesAsync<T>(this IEnumerable<T> sequence)
        {
            return Task.Run(() => sequence.ToStringLines());
        }

        public static Task<string> ToStringLinesAsync<T>(this IEnumerable<T> sequence, Func<T, string> toString)
        {
            return Task.Run(() => sequence.ToStringLines(toString));
        }
    }
}
