using System;
using System.Collections.Generic;
using Optional;
using Optional.Unsafe;

namespace TriResultsCsvReader
{
    public class Race
    {
        public Race()
        {
            Date = Option.None<DateTime>();
            Name = Option.None<String>();
            Distance = Option.None<String>();
            RaceType = Option.None<String>();
            Results = new List<ResultRow>();
        }

        public Option<string> Name { get; set; }
        public Option<DateTime> Date { get; set; }
        public Option<string> Distance { get; set; }
        public Option<string> RaceType { get; set; }
        public List<ResultRow> Results { get; set; }

        public string ToFilename()
        {
            return string.Format("{0}_{1}", Date == null ? "0000" : Date.ValueOrDefault().ToString("yyyyMMdd"), Name == null ? "___" : Name.ValueOrDefault().Replace(" ", "_"));
        }
    }

    
}
