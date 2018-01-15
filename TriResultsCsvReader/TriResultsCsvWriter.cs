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

        public void StandardizeCsv(string raceName, string srcFile, string destPath, Expression<Func<ResultRow, bool>> filterExpression = null)
        {
            var reader = new ResultsReaderCsv(_readerConfigXml, _outputWriter);

            var resultRows = reader.ReadFile(srcFile, filterExpression).ToList();

            var raceType = SetRaceType(resultRows);

            WriteOutput($"Read {resultRows.Count()} rows of {raceType}\n");

            if (_skipEmptyResults && resultRows.Any())
            {
                var raceDate = resultRows.First().RaceDate;
                Write(destPath, raceDate, raceName, resultRows, raceType);
            }
        }

        public string SetRaceType(List<ResultRow> results)
        {
            var guesser = new RaceTypeGuesser();

            var raceType = guesser.GetRaceType(results.FirstOrDefault());
            foreach(var result in results)
            {
                result.RaceType = raceType;
            }

            return raceType;
        }

        public void Write(string destFolder, DateTime raceDate, string raceName, IEnumerable<ResultRow> rows, string raceType)
        {
            foreach (var row in rows)
            {
                if (string.IsNullOrEmpty(row.Race)) {
                    row.Race = raceName;
                }
            }

            var dir = new DirectoryInfo(destFolder);
            if(!Directory.Exists(dir.FullName))
            {
                Console.WriteLine("Creating dir: " + dir.FullName);
                Directory.CreateDirectory(dir.FullName);
            }

            var csvReaderConfig = new Configuration() { HeaderValidated = null, SanitizeForInjection = false, TrimOptions = TrimOptions.Trim };

            var filename = String.Format("{0}_{1}_{2}.csv", raceDate.ToString("yyyy-MM-dd"), raceType, raceName); 
            var destFile = Path.Combine(destFolder, filename);
            Console.WriteLine("destFile: " + destFile);

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
