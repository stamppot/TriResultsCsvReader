using System;
using System.Linq;
using Optional;

namespace UrlResultsFetcher
{
    public class DateUtils
    {
        public static Option<Tuple<string,DateTime>> FromRaceData(string racedata)
        {
            var dtStr = ReplaceStringMonth(racedata);
            var dateIndex = FindFirstNumberIndex(dtStr);
            dtStr = racedata.Substring(dateIndex);

            var raceStr = racedata.Substring(0, dateIndex).Trim();

            if (dtStr.EndsWith("-"))
            {
                dtStr = dtStr.Substring(0, dtStr.Length - 1);
            }

            DateTime output = DateTime.MinValue;

            if (DateTime.TryParse(dtStr, out output))
            {
                return Option.Some(new Tuple<string, DateTime>(raceStr, output));
            }

            return Option.None<Tuple<string,DateTime>>();
        }

        public static int FindFirstNumberIndex(string str)
        {
            var chars = str.AsEnumerable().ToArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (Char.IsNumber(chars[i]))
                {
                    return i;
                }
            }

            return -1;
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
