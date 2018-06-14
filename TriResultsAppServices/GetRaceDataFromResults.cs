using System;
using Optional;
using TriResultsCsvReader;

namespace TriResultsDomainServices
{
    public class GetRaceDataFromResults
    {
        public RaceEnvelope FillRaceEnvelope(RaceEnvelope raceStepData, Option<ResultRow> raceResultRow)
        {
            if (!raceResultRow.HasValue)
                return raceStepData;

            // get raceType

            var resultRow = raceResultRow.ValueOr(new ResultRow());

            if (!raceStepData.RaceData.Date.HasValue)
            {
                raceStepData.RaceData.Date = null == resultRow.RaceDate ? Option.None<DateTime>() : Option.Some(resultRow.RaceDate);
            }

            // if resultrow has Race column, use this as it is a better description of the race name
            if (!raceStepData.RaceData.Name.HasValue || !string.IsNullOrEmpty(resultRow.Race))
            {
                raceStepData.RaceData.Name = Option.Some(resultRow.Race);
            }

            if (!raceStepData.RaceData.RaceType.HasValue)
            {
                raceStepData.RaceData.RaceType = string.IsNullOrEmpty(resultRow.RaceType)
                    ? Option.None<string>()
                    : Option.Some(resultRow.RaceType);
            }

            return raceStepData;
        }
    }
}
