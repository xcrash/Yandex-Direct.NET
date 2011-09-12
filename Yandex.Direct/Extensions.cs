using System;
using System.Collections.Generic;
#if NET4
using System.Diagnostics.Contracts;
#endif
using System.Linq;
using System.Text;

namespace Yandex.Direct
{
    internal static class Extensions
    {
		public static string Merge(this IEnumerable< string> strings)
		{
			return Merge(strings, null);
		}

        public static string Merge(this IEnumerable< string> strings, string separator)
        {
#if NET4
            Contract.Requires(strings!= null);
#else
			if (strings == null) throw new ArgumentNullException("strings");
#endif
            if (separator == null)
                return strings.Aggregate(new StringBuilder(), (x, y) => x.Append(y)).ToString();
            var stringBuilder = strings.Aggregate(new StringBuilder(), (x, y) => x.Append(y).Append(separator));
            return (stringBuilder.Length >= separator.Length)
                       ? stringBuilder.ToString(0, stringBuilder.Length - separator.Length)
                       : stringBuilder.ToString();

        }
    }
}