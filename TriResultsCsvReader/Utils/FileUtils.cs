using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optional;

namespace TriResultsCsvReader.Utils
{
    public class FileUtils
    {
        public FileUtils(bool debug = false)
        {
            Debug = debug;
        }

        public bool Debug { get; set; }

        public List<string> GetAllFiles(string inputFileOrFolder, bool printVerboseOutput = true)
        {
            var files = new List<string>();

            var dirInfo = new DirectoryInfo(inputFileOrFolder);

            if (printVerboseOutput || Debug)
            {
                Console.WriteLine($"Input {inputFileOrFolder}");
            }

            if (dirInfo.Exists)
            {
                Console.WriteLine($"Is a folder {inputFileOrFolder}");
                // a directory is given
                files.AddRange(dirInfo.GetFiles("*.csv", SearchOption.TopDirectoryOnly).Select(f => f.FullName).Where(f => !f.EndsWith("_output.csv")));
            }
            else
            {
                Console.WriteLine($"Is a file {inputFileOrFolder}");
                // a file is given
                var fileInfo = new FileInfo(inputFileOrFolder);
                if (!fileInfo.Exists) { var errorMessage = $"Input file does not exist: {inputFileOrFolder}"; Console.WriteLine(errorMessage); throw new FileNotFoundException(errorMessage); }

                files.Add(fileInfo.FullName);
            }

            if (printVerboseOutput || Debug)
            {
                Console.WriteLine("Found files:\n");
                files.ForEach(f => Console.WriteLine($"{f}"));
            }

            return files;
        }

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
