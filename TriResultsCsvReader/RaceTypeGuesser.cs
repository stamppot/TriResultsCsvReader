namespace TriResultsCsvReader
{
    public class RaceTypeGuesser
    {
        public string GetRaceType(ResultRow row)
        {
            if (row == null) return string.Empty;

            if(IsATriathlon(row))
            {
                return "Tri";
            }
            if(IsARunBikeRun(row))
            {
                return "RBR";
            }
            if(IsARun(row))
            {
                return "Run";
            }
            if(HasSwimData(row))
            {
                return "Swim";
            }

            return "-";
        }

        public bool IsARunBikeRun(ResultRow row)
        {
            return !HasSwimData(row) && (HasRunData(row) || HasTwoRunData(row));
        }

        public bool IsATriathlon(ResultRow row)
        {
            return HasSwimData(row) && HasBikeData(row) && HasRunData(row);
        }

        public bool IsARun(ResultRow row)
        {
            return !HasSwimData(row) && !HasBikeData(row); 
        }

        private bool HasSwimData(ResultRow result)
        {
            return !string.IsNullOrWhiteSpace(result.Swim) || result.PosSwim.HasValue;
        }

        private bool HasBikeData(ResultRow result)
        {
            return !string.IsNullOrEmpty(result.Bike) || !string.IsNullOrEmpty(result.AfterBike) || result.PosBike.HasValue || result.PosAfterBike.HasValue;
        }

        private bool HasRunData(ResultRow result)
        {
            return !string.IsNullOrEmpty(result.Run) || result.PosRun.HasValue;
        }

        private bool HasTwoRunData(ResultRow result)
        {
            return !string.IsNullOrEmpty(result.Run1) || !string.IsNullOrEmpty(result.Run2) || result.PosRun1.HasValue || result.PosRun2.HasValue;
        }
    }
}
