using System;
using System.Linq;
using System.Text;
using System.IO;

namespace FileAppServices
{
    public class ValidateCsvNumberOfColumns
    {
        private char separator = ',';

        public bool Validate(string csvFilename)
        {
            var csvLines = File.ReadAllLines(csvFilename);

            return Validate(csvLines);
        }

        public bool Validate(string[] csvLines)
        {
            var linesInfo = 
            csvLines.Select((line, index) => { var hasQuote = line.Contains("\""); var cols = line.Split(separator); return new { RowNumber = index+1, Columns = cols, NumColumns = cols.Count(), IsQuoted = hasQuote }; }).ToList();

            var header = linesInfo.First();
            var headerColumnNum = header.NumColumns;

            var hasQuotedFields = linesInfo.Any(li => li.IsQuoted);
            
            if(hasQuotedFields)
            {
                Console.WriteLine("Has quoted fields, cannot validate column numbers");
                return true;
            }

            var hasSameAmountOfColumns = linesInfo.All(li => li.NumColumns == headerColumnNum);

            if(!hasSameAmountOfColumns)
            {
                var sb = new StringBuilder();
                sb.AppendLine(String.Format("Header:  Columns: {0}, {1}", header.NumColumns, string.Join(separator.ToString(), header.Columns)));

                foreach(var li in linesInfo.Where(li => li.NumColumns != headerColumnNum))
                {
                    sb.AppendLine(String.Format("{0} Row: columns: {1}, : {2}", li.RowNumber, li.NumColumns, string.Join(separator.ToString(), li.Columns)));
                }

                throw new FormatException("Not equal amount of columns:" + Environment.NewLine + sb.ToString());
            }

            return hasSameAmountOfColumns;
        }
    }
}
