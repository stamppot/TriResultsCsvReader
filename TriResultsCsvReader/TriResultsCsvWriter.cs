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
        private readonly Action<string> _outputWriter;
        private readonly string _dateFormat = "yyyy-MM-dd";
        private string _readerConfigXml;
        private bool _skipEmptyResults;

        public TriResultsCsvWriter(string columnConfigXmlPath, Action<string> outputWriter, bool skipEmptyResults = true)
        {
            _outputWriter = outputWriter;
            _readerConfigXml = columnConfigXmlPath;
            _skipEmptyResults = skipEmptyResults;
        }

        public void StandardizeCsv(string raceName, DateTime raceDate, string srcFile, string destFile, Expression<Func<ResultRow, bool>> filterExpression = null)
        {
            var reader = new ResultsReaderCsv(_readerConfigXml, _outputWriter);

            var resultRows = reader.ReadFile(srcFile, filterExpression);

            WriteOutput($"Read {resultRows.Count()} rows");

            if (_skipEmptyResults && resultRows.Any())
            {
                Write(destFile, raceName, raceDate, resultRows);
            }
        }

        public void Write(string destFile, string raceName, DateTime raceDate, IEnumerable<ResultRow> rows)
        {
            foreach (var row in rows)
            {
                row.Race = raceName;
                row.RaceDate = raceDate.ToString(_dateFormat);
            }

            var csvReaderConfig = new Configuration() { HeaderValidated = null, SanitizeForInjection = false, TrimOptions = TrimOptions.Trim };

            using (TextWriter writer = new StreamWriter(destFile))
            {
                var csvWriter = new CsvWriter(writer);

                csvWriter.WriteRecords<ResultRow>(rows);
            }
        }

        private void WriteOutput(string message)
        {
            if(null != _outputWriter)
            {
                _outputWriter.Invoke(message);
            }
        }
    }
}
