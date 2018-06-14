using Optional;
using TriResultsCsvReader;

namespace TriResultsDomainService
{
    public class RaceDataFileUtils
    {
        public RaceDataFileUtils(bool debug = false)
        {
            Debug = debug;
        }

        public bool Debug { get; set; }

        public Option<Race> GetRaceDataFromFilename(string filename)
        {
            var file = filename.Substring(filename.LastIndexOf('\\') + 1);

            var raceDate = DateUtils.FromFilename(filename);
            var raceName = Option.None<string>();

            var raceGuesser = new RaceTypeGuesser();

            var raceType = raceGuesser.GetRaceType(filename);
            var distance = raceGuesser.GetRaceDistance(filename);

            if (raceDate.HasValue)
            {
                for (int i = 0; i < 3; i++)
                {
                    file = file.Substring(file.IndexOf("-") + 1);
                }
                raceName = Option.Some<string>(file.Replace(".csv", "").Replace("-", " ").Replace("_", ""));

                return Option.Some(new Race() { Date = raceDate, Name = raceName, RaceType = raceType, Distance = distance });
            }


            return Option.None<Race>();
        }
    }
}
