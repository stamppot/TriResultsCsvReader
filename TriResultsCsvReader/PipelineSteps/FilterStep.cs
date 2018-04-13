using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TriResultsCsvReader.PipelineSteps
{

    public class FilterStep : BaseStep, IPipelineStep
    {
        private readonly Expression<Func<ResultRow, bool>> _filterExp;
        private readonly IEnumerable<Column> _columns;
        private readonly List<string> _infoLogs;

        public FilterStep(IEnumerable<Column> columnsConfig, Expression<Func<ResultRow, bool>> filterExp,
            List<string> infoLogs)
        {
            _columns = columnsConfig;
            _filterExp = filterExp;
            // is add only, read by caller
            _infoLogs = infoLogs;
        }

        public IEnumerable<Column> GetColumns()
        {
            return _columns;
        }

        public override RaceStepData Process(RaceStepData step)
        {
            // filtering happens here
            var allResults = step.RaceData.Results;


            _infoLogs.Add($"Input {step.RaceData.Name} {step.RaceData.Results.Count()} results\n" + Environment.NewLine);

            if (_filterExp != null)
            {
                var compiledFilter = _filterExp.Compile();
                step.RaceData.Results = allResults.Where(r => compiledFilter.Invoke(r)).ToList();
            }

            _infoLogs.Add($"Filtered output {step.RaceData.Results.Count()} rows\n" + Environment.NewLine);

            return step;
        }
    }
}