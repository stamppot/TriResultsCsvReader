namespace TriResultsCsvReader
{
    public class RaceTypeGuesser
    {
        public string GetRaceDistance(string name)
        {
            var str = name.ToLower();

            if(str.Contains("achtste") || str.Contains("acht") || str.Contains("8e") || str.Contains("8ste") || str.Contains("1/8"))
            {
                return "Achtste";
            }
            if (str.Contains("sprint") )
            {
                return "Sprint";
            }
            if (str.Contains("kwart") || str.Contains("1/4"))
            {
                return "Kwart";
            }

            if (name.Contains("OD") || str.Contains("olymp"))
            {
                return "OD";
            }

            if (name.Contains("MD") || name.Contains("Mid") || str.Contains("halve") || str.Contains("half"))
            {
                return "Half";
            }

            if (name.Contains("LD") || str.Contains("full") || str.Contains("long"))
            {
                return "Long";
            }
            
            // run
            if (str.Contains("42k") || str.Contains("marathon") || str.Contains("42"))
            {
                return "Marathon";
            }

            if (str.Contains("21k") || str.Contains("21"))
            {
                return "Half Marathon";
            }

            if (str.Contains("5k"))
            {
                return "5km";
            }
            if (str.Contains("6k"))
            {
                return "6km";
            }

            if (str.Contains("9k"))
            {
                return "9km";
            }

            if (str.Contains("10k"))
            {
                return "10km";
            }

            if (str.Contains("12k"))
            {
                return "12km";
            }

            if (str.Contains("15k"))
            {
                return "15km";
            }
            if (str.Contains("16k"))
            {
                return "16km";
            }

            if (str.Contains("20k"))
            {
                return "20km";
            }

            if (str.Contains("4k"))
            {
                return "4km";
            }


            if (str.Contains("10em"))
            {
                return "10 Engelse Mijl";
            }

            return "";
        }

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
