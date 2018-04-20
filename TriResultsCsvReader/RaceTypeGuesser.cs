using Optional;

namespace TriResultsCsvReader
{
    public class RaceTypeGuesser
    {
        public Option<string> GetRaceType(string filename)
        {
            return new FilenameRaceTypeGuesser(filename).GetRaceType();
        }

        public bool GetRaceType(string filename, out string raceType)
        {
            raceType = new FilenameRaceTypeGuesser(filename).GetRaceType().ValueOr(string.Empty);
            return !string.IsNullOrEmpty(raceType);
        }

        public bool GetRaceType(ResultRow row, out string raceType)
        {
            raceType = new RowRaceTypeGuesser(row).GetRaceType().ValueOr(string.Empty);
            return !string.IsNullOrEmpty(raceType);
        }

        public Option<string> GetRaceType(ResultRow row)
        {
            return new RowRaceTypeGuesser(row).GetRaceType();
        }

        public Option<string> GetRaceDistance(string name)
        {
            var str = name.ToLower();
            string result = null;

            if (str.Contains("achtste") || str.Contains("acht") || str.Contains("8e") || str.Contains("8ste") ||
                str.Contains("1/8"))
            {
                result = "Achtste";
            }

            if (str.Contains("sprint"))
            {
                result = "Sprint";
            }

            if (str.Contains("kwart") || str.Contains("1/4"))
            {
                result = "Kwart";
            }

            if (name.Contains("OD") || str.Contains("olymp"))
            {
                result = "OD";
            }

            if (name.Contains("MD") || name.Contains("Mid") || str.Contains("halve") || str.Contains("half"))
            {
                result = "Half";
            }

            if (name.Contains("LD") || str.Contains("full") || str.Contains("long"))
            {
                result = "Long";
            }

            // run
            if (str.Contains("42k") || str.Contains("marathon") || str.Contains("42"))
            {
                result = "Marathon";
            }

            if (str.Contains("21k") || str.Contains("21"))
            {
                result = "Half Marathon";
            }

            if (str.Contains("4k"))
            {
                result = "4km";
            }

            if (str.Contains("5k"))
            {
                result = "5km";
            }

            if (str.Contains("6k"))
            {
                result = "6km";
            }

            if (str.Contains("8k"))
            {
                result = "8km";
            }

            if (str.Contains("9k"))
            {
                result = "9km";
            }

            if (str.Contains("10k"))
            {
                result = "10km";
            }

            if (str.Contains("12k"))
            {
                result = "12km";
            }

            if (str.Contains("15k"))
            {
                result = "15km";
            }

            if (str.Contains("16k"))
            {
                result = "16km";
            }

            if (str.Contains("20k"))
            {
                result = "20km";
            }

            if (str.Contains("10em"))
            {
                result = "10 Engelse Mijl";
            }

            return result == null ? Option.None<string>() : Option.Some(result);
        }
    }


    public interface IRaceTypeGuesser
    {
        Option<string> GetRaceType();
    }

    public class RowRaceTypeGuesser : IRaceTypeGuesser
    {
        private readonly ResultRow row;

        public RowRaceTypeGuesser(ResultRow row)
        {
            this.row = row;
        }

        public Option<string> GetRaceType()
        {
            if (null == row) return Option.None<string>();

            string result = null;

            if (IsATriathlon(row))
            {
                result = "Tri";
            }

            if (IsARunBikeRun(row))
            {
                result = "RBR";
            }

            if (IsARun(row))
            {
                result = "Run";
            }

            if (HasSwimData(row))
            {
                result = "Swim";
            }

            return result == null ? Option.None<string>() : Option.Some(result);
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
            return !string.IsNullOrEmpty(result.Bike) || !string.IsNullOrEmpty(result.AfterBike) ||
                   result.PosBike.HasValue || result.PosAfterBike.HasValue;
        }

        private bool HasRunData(ResultRow result)
        {
            return !string.IsNullOrEmpty(result.Run) || result.PosRun.HasValue;
        }

        private bool HasTwoRunData(ResultRow result)
        {
            return !string.IsNullOrEmpty(result.Run1) || !string.IsNullOrEmpty(result.Run2) ||
                   result.PosRun1.HasValue || result.PosRun2.HasValue;
        }

    }


    public class FilenameRaceTypeGuesser : IRaceTypeGuesser
    {
        private readonly string _filename;

        public FilenameRaceTypeGuesser(string filename)
        {
            _filename = filename;
        }
        public Option<string> GetRaceType()
        {
            if (_filename == null) return Option.None<string>();

            string result = null;

            if (IsATriathlon())
            {
                result = "Tri";
            }
            else
            if (IsARunBikeRun())
            {
                result = "RBR";
            }
            else
            if (IsARun())
            {
                result = "Run";
            }
            else
            if (HasSwimData())
            {
                result = "Swim";
            }

            return result == null ? Option.None<string>() : Option.Some(result);
        }

        public bool IsARunBikeRun()
        {
            return !string.IsNullOrEmpty(_filename) && (_filename.ToLowerInvariant().Contains("rbr")); ;
        }

        public bool IsATriathlon()
        {
            return _filename.ToLowerInvariant().Contains("tri") || HasSwimData() && HasBikeData() && HasRunData();
        }

        public bool IsARun()
        {
            return !HasSwimData() && !HasBikeData();
        }

        private bool HasSwimData()
        {
            return !string.IsNullOrWhiteSpace(_filename) && _filename.ToLowerInvariant().Contains("swim");
        }

        private bool HasBikeData()
        {
            return !string.IsNullOrEmpty(_filename) && _filename.ToLowerInvariant().Contains("bike");
        }

        private bool HasRunData()
        {
            return !string.IsNullOrEmpty(_filename) && (_filename.ToLowerInvariant().Contains("loop") || _filename.ToLowerInvariant().Contains("run"));
        }

        private bool HasTwoRunData()
        {
            return !string.IsNullOrEmpty(_filename) && (_filename.ToLowerInvariant().Contains("rbr"));
        }
    }
}
