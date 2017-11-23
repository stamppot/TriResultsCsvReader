using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using CsvHelper;
using CsvHelper.Configuration;

namespace TriResultsCsvReader
{
    public class TriResultsCsvWriter
    {
        private readonly string _dateFormat = "yyyy-MM-dd";
        private string _readerConfigXml;

        public TriResultsCsvWriter(string columnConfigXmlPath)
        {
            _readerConfigXml = columnConfigXmlPath;
        }

        public void StandardizeCsv(string raceName, DateTime raceDate, string srcFile, string destFile, Expression<Func<ResultRow, bool>> filterExpression = null)
        {
            var reader = new ResultsReaderCsv(_readerConfigXml);

            var resultRows = reader.ReadFile(srcFile, filterExpression);

            Write(destFile, raceName, raceDate, resultRows);
        }

        public void Write(string destFile, string raceName, DateTime raceDate, IEnumerable<ResultRow> rows)
        {
            foreach (var row in rows)
            {
                row.Race = raceName;
                row.RaceDate = raceDate.ToString(_dateFormat);
            }

            var csvReaderConfig = new Configuration() { HeaderValidated = null, MissingFieldFound = null, SanitizeForInjection = true, TrimOptions = TrimOptions.Trim };

            using (TextWriter writer = new StreamWriter(destFile))
            {
                var csvReader = new CsvWriter(writer);
            }
        }
    }
}
