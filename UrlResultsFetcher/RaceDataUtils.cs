using System;
using System.Globalization;
using Optional;

namespace UrlResultsFetcher
{
    public class RaceDataUtils
    {
        public static Option<Tuple<string, DateTime>> FromRaceData(string racedata)
        {
            var dtStr = DateUtils.ReplaceStringMonth(racedata);
            var dateIndex = DateUtils.FindFirstNumberIndex(dtStr);
            dtStr = racedata.Substring(dateIndex);

            var raceStr = racedata.Substring(0, dateIndex).Trim();

            if (dtStr.EndsWith("-"))
            {
                dtStr = dtStr.Substring(0, dtStr.Length - 1);
            }

            DateTime output = DateTime.MinValue;

            if (DateTime.TryParseExact(dtStr, "dd-MM-yyyy", null, DateTimeStyles.None, out output) || DateTime.TryParse(dtStr, out output))
            {
                return Option.Some(new Tuple<string, DateTime>(raceStr, output));
            }

            return Option.None<Tuple<string, DateTime>>();
        }
    }
}
