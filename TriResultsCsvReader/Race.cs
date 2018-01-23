using System;
using System.Collections.Generic;

namespace TriResultsCsvReader
{
    public class Race
    {
        public Race()
        {
            Date = DateTime.Now;
            Name = string.Empty;
            Results = new List<ResultRow>();
        }

        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Distance { get; set; }
        public string RaceType { get; set; }
        public List<ResultRow> Results { get; set; }

        public string ToFilename()
        {
            return string.Format("{0}_{1}", Date == null ? "0000" : Date.ToString("yyyyMMdd"), Name == null ? "___" : Name.Replace(" ", "_"));
        }
    }

    
}
