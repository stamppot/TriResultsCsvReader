using System.Collections.Generic;

namespace TriResultsCsvReader
{
    public class StepData
    {
        public StepData()
        {
            RaceData = new Race();
        }

        public Race RaceData { get; set; }

        public string InputFile { get; set; }
        public string FullPath { get; set; }  // one of these should be made redundant

        public List<string> OutputOptions { get; set; }
        
        public string OutputFolder { get; set; }

        public IEnumerable<Column> ColumnConfig { get; set; }
    }
}
