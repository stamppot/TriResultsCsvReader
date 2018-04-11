using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Optional;
using Optional.Unsafe;

namespace TriResultsCsvReader
{
    public class StandardizeHeadersAndFilterStep : BaseStep, IPipelineStep
    {
        private readonly Expression<Func<ResultRow, bool>> _filterExp;
        private readonly IEnumerable<Column> _columns;

        public StandardizeHeadersAndFilterStep(IEnumerable<Column> columnsConfig, Expression<Func<ResultRow, bool>> filterExp)
        {
            _columns = columnsConfig;
            _filterExp = filterExp;
        }

        public IEnumerable<Column> GetColumns()
        {
            return _columns;
        }

        public override StepData Process(StepData step)
        {
            var reader = new ResultsReaderCsv(GetColumns(), WriteOutput);

            // filtering happens here
            var resultRows = reader.ReadFile(step.InputFile, _filterExp).ToList();

            WriteOutput($"Read {resultRows.Count()} rows\n");

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
                    step.RaceData.RaceType = string.IsNullOrEmpty(firstResult.RaceType) ? Option.None<string>() : Option.Some(firstResult.RaceType);
                }

                step.RaceData.Results = resultRows;
                WriteOutput($"From race {step.RaceData.Name}  {firstResult.Race}\n");
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
