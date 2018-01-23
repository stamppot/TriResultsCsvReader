using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using CsvHelper;
using CsvHelper.Configuration;

namespace TriResultsCsvReader
{
    public class StandardizeHeadersAndFilterStep : BaseStep, IPipelineStep
    {
        private readonly string _dateFormat = "yyyy-MM-dd";
        private string _readerConfigXml;
        private bool _skipEmptyResults;
        private readonly IEnumerable<Column> _columns;
        private readonly RaceTypeGuesser _raceGuesser = new RaceTypeGuesser();

        public StandardizeHeadersAndFilterStep(IEnumerable<Column> columns, bool skipEmptyResults = true)
        {
            _columns = columns;
            _skipEmptyResults = skipEmptyResults;
        }

        public IEnumerable<Column> GetColumns()
        {
            return _columns;
        }

        public override StepData Process(StepData step)
        {
            var reader = new ResultsReaderCsv(GetColumns(), WriteOutput);

            // filtering happens here
            var resultRows = reader.ReadFile(step.InputFile, step.Filter).ToList();

            step.RaceData.Distance = _raceGuesser.GetRaceDistance(step.InputFile);
            var raceType = SetRaceType(resultRows);
            step.RaceData.RaceType = raceType;

            WriteOutput($"Read {resultRows.Count()} rows of {raceType}\n");

            if (resultRows.Any())
            {
                var firstResult = resultRows.First();
                step.RaceData.Date = firstResult.RaceDate;
                step.RaceData.Name = firstResult.Race;
                step.RaceData.RaceType = firstResult.RaceType;
                step.RaceData.Results = resultRows;
                WriteOutput($"From race {step.RaceData.Name}  {firstResult.Race}\n");
            }

            return step;
        }

        //public void StandardizeCsv(string raceName, string srcFile, string destPath, Expression<Func<ResultRow, bool>> filterExpression = null)
        //{
        //    var reader = new ResultsReaderCsv(_readerConfigXml, WriteOutput);

        //    var resultRows = reader.ReadFile(srcFile, filterExpression).ToList();

        //    var raceType = SetRaceType(resultRows);

        //    WriteOutput($"Read {resultRows.Count()} rows of {raceType}\n");

        //    if (_skipEmptyResults && resultRows.Any())
        //    {
        //        var raceDate = resultRows.First().RaceDate;
        //        Write(destPath, raceDate, raceName, resultRows, raceType);
        //    }
        //}

        public string SetRaceType(List<ResultRow> results)
        {
            var firstResult = results.FirstOrDefault();
            var raceType = _raceGuesser.GetRaceType(firstResult);

            foreach(var result in results)
            {
                result.RaceType = raceType;
            }

            return raceType;
        }

        //[Obsolete("Is done in another step")]
        //public void Write(string destFolder, DateTime raceDate, string raceName, IEnumerable<ResultRow> rows, string raceType)
        //{
        //    foreach (var row in rows)
        //    {
        //        if (string.IsNullOrEmpty(row.Race)) {
        //            row.Race = raceName;
        //        }
        //    }

        //    var dir = new DirectoryInfo(destFolder);
        //    if(!Directory.Exists(dir.FullName))
        //    {
        //        Console.WriteLine("Creating dir: " + dir.FullName);
        //        Directory.CreateDirectory(dir.FullName);
        //    }

        //    var csvReaderConfig = new Configuration() { HeaderValidated = null, SanitizeForInjection = false, TrimOptions = TrimOptions.Trim };

        //    var filename = String.Format("{0}_{1}_{2}.csv", raceDate.ToString("yyyy-MM-dd"), raceType, raceName); 
        //    var destFile = Path.Combine(destFolder, filename);
        //    Console.WriteLine("destFile: " + destFile);

        //    using (TextWriter writer = new StreamWriter(destFile))
        //    {
        //        var csvWriter = new CsvWriter(writer);

        //        csvWriter.WriteRecords<ResultRow>(rows);
        //    }
        //}
    }
}
