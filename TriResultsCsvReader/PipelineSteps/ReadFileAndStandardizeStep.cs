using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Unsafe;

namespace TriResultsCsvReader
{
    public class ReadFileAndStandardizeStep : BaseStep, IPipelineStep
    {
        private readonly IEnumerable<Column> _columns;

        public ReadFileAndStandardizeStep(IEnumerable<Column> columnsConfig, List<string> infoLogs = null) : base(infoLogs)
        {
            _columns = columnsConfig;
        }

        public IEnumerable<Column> GetColumns()
        {
            return _columns;
        }

        public override RaceStepData Process(RaceStepData step)
        {
            var reader = new ResultsReaderCsv(GetColumns(), WriteInfo);

            // filtering happens here
            var resultRows = reader.ReadFile(step.InputFile).ToList();

            WriteInfo($"Read {resultRows.Count()} rows\n");

            if (resultRows.Any())
            {
                var firstResult = resultRows.First();

                // get raceType

                if (!step.RaceData.Date.HasValue)
                {
                    step.RaceData.Date = Option.Some(firstResult.RaceDate);
                }

                // if resultrow has Race column, use this as it is a better description of the race name
                if (!step.RaceData.Name.HasValue || !string.IsNullOrEmpty(firstResult.Race))
                {
                    step.RaceData.Name = Option.Some(firstResult.Race);
                }

                if (!step.RaceData.RaceType.HasValue)
                {
                    step.RaceData.RaceType = string.IsNullOrEmpty(firstResult.RaceType) ? Option.None<string>() : Option.Some(firstResult.RaceType);
                }


                step.RaceData.Results = resultRows.ToList();
                WriteInfo($"From race {step.RaceData.Name}  {firstResult.Race}\n");
            }

            return step;
        }

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
    }
}
