using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Unsafe;

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

            var raceFileData = GetRaceDataFromFilename(step.InputFile);
            if (raceFileData.HasValue)
            {
                step.RaceData = raceFileData.ValueOrDefault();
            }

            // filtering happens here
            var resultRows = reader.ReadFile(step.InputFile, step.Filter).ToList();

            if (_raceGuesser.GetRaceType(step.InputFile, out var raceType) || _raceGuesser.GetRaceType(resultRows.First(), out raceType))
            {
                step.RaceData.RaceType = SetRaceType(resultRows, step.RaceData.RaceType);
            }

            step.RaceData.Distance = _raceGuesser.GetRaceDistance(step.InputFile);
            
            WriteOutput($"Read {resultRows.Count()} rows of {raceType}\n");

            if (resultRows.Any())
            {
                var firstResult = resultRows.First();

                // get raceType

                if (!step.RaceData.Date.HasValue)
                {
                    step.RaceData.Date = Option.Some(firstResult.RaceDate);
                }

                if (!step.RaceData.Name.HasValue)
                {
                    step.RaceData.Name = Option.Some(firstResult.Race);
                }

                if (!step.RaceData.RaceType.HasValue)
                {
                    step.RaceData.RaceType = Option.Some(firstResult.RaceType);
                }

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

        public Option<string> SetRaceType(List<ResultRow> results, Option<string> raceType)
        {
            if (results.Any())
            {
                var first = results.First(); // if set, use this, otherwise use raceType from filename


                var raceTypeResult = !string.IsNullOrEmpty(first.Race) ? first.RaceType : raceType.ValueOrDefault();

                foreach (var result in results)
                {
                    if (string.IsNullOrEmpty(result.RaceType))
                        result.RaceType = raceType.ValueOrDefault();
                }

                return Option.Some(raceTypeResult);
            }

            return raceType;
        }

        protected Option<Race> GetRaceDataFromFilename(string filename)
        {
            var file = filename.Substring(filename.LastIndexOf('\\') + 1);

            var raceDate = DateUtils.FromFilename(filename);
            var raceName = Option.None<string>();
            var raceType = _raceGuesser.GetRaceType(filename);
            var distance = _raceGuesser.GetRaceDistance(filename);

            if (raceDate.HasValue)
            {
                for(int i = 0; i < 3; i++)
                {
                    file = file.Substring(file.IndexOf("-") + 1);
                }
                raceName = Option.Some<string>(file.Replace(".csv", "").Replace("-", " ").Replace("_", ""));

                return Option.Some(new Race() { Date = raceDate, Name = raceName, RaceType = raceType, Distance = distance});
            }

            return Option.None<Race>();
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
