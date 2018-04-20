using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

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

        public override RaceEnvelope Process(RaceEnvelope raceStepData)
        {
            var reader = new ResultsReaderCsv(GetColumns(), WriteInfo);

            // filtering happens here
            var resultRows = reader.ReadFile(raceStepData.InputFile).ToList();

            if (resultRows.Any())
            {
                var firstResult = resultRows.First();

                // get raceType

                if (!raceStepData.RaceData.Date.HasValue)
                {
                    raceStepData.RaceData.Date = Option.Some(firstResult.RaceDate);
                }

                // if resultrow has Race column, use this as it is a better description of the race name
                if (!raceStepData.RaceData.Name.HasValue || !string.IsNullOrEmpty(firstResult.Race))
                {
                    raceStepData.RaceData.Name = Option.Some(firstResult.Race);
                }

                if (!raceStepData.RaceData.RaceType.HasValue)
                {
                    raceStepData.RaceData.RaceType = string.IsNullOrEmpty(firstResult.RaceType) ? Option.None<string>() : Option.Some(firstResult.RaceType);
                }


                raceStepData.RaceData.Results = resultRows.ToList();
                WriteInfo($"Read {resultRows.Count()} rows from race {raceStepData.RaceData.Name}  {firstResult.Race}\n");
            }

            return raceStepData;
        }
    }
}
