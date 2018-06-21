using System;
using System.Linq;
using Optional;

namespace UrlResultsFetcher
{
    public class DateUtils
    {
        public static Option<int> FindFirstNumberIndex(string str)
        {
            var chars = str.AsEnumerable().ToArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (Char.IsNumber(chars[i]))
                {
                    // try to check the next number to avoid fx "4e Zeewolde Endurance 25-06-2018
                    if (i < chars.Length && (Char.IsNumber(chars[i + 1]) || chars[i + 1] == '-'))
                    {
                        return Option.Some(i);
                    }
                }
            }

            return Option.None<int>();
        }

        public static string ReplaceStringMonth(string name)
        {
            return name
                    .Replace("-jan-", "-01-")
                    .Replace("-feb-", "-02-")
                    .Replace("-mar-", "-03-")
                    .Replace("-apr-", "-04-")
                    .Replace("-may-", "-05-")
                    .Replace("-jun-", "-06-")
                    .Replace("-jul-", "-07-")
                    .Replace("-aug-", "-08-")
                    .Replace("-sep-", "-09-")
                    .Replace("-oct-", "-10-")
                    .Replace("-nov-", "-11-")
                    .Replace("-dec-", "-12-")
                    .Replace("-mei-", "-05-")
                    .Replace("-okt-", "-10-")
                    .Replace("-Jan-", "-01-")
                    .Replace("-Feb-", "-02-")
                    .Replace("-Mar-", "-03-")
                    .Replace("-Apr-", "-04-")
                    .Replace("-May-", "-05-")
                    .Replace("-Jun-", "-06-")
                    .Replace("-Jul-", "-07-")
                    .Replace("-Aug-", "-08-")
                    .Replace("-Sep-", "-09-")
                    .Replace("-Oct-", "-10-")
                    .Replace("-Nov-", "-11-")
                    .Replace("-Dec-", "-12-")
                ;
        }
    }
}
