using System;
using Optional;

namespace TriResultsCsvReader
{
    public class DateUtils
    {
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
                .Replace("-dec-", "-12-");
        }

        public static string ToFilenameFormat(DateTime dt)
        {
            return dt.ToString("yyyy-MMM-dd");
        }

        public static Option<DateTime> FromFilename(string filename)
        {
            filename = filename.Substring(1 + filename.LastIndexOf('\\'));
            var dtStr = ReplaceStringMonth(filename).Substring(0, 10);

            DateTime output = DateTime.MinValue;

                if (DateTime.TryParse(dtStr, out output))
                {
                    return Option.Some<DateTime>(output);
                }

            return Option.None<DateTime>();
        }

        public static string ToRaceFilename(DateTime dt, string raceName)
        {
            return ToFilenameFormat(dt) + "_" + raceName.Replace(" ", "-");
        }
    }
}
