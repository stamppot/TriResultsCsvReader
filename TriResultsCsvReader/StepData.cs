using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
        
        public string OutputFile { get; set; }

        public Expression<Func<ResultRow, bool>> Filter { get; set; }


        public string ColumnConfigFile { get; set; }
        public IEnumerable<Column> ColumnConfig { get; set; }
    }
}
