using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optional.Unsafe;

namespace TriResultsCsvReader
{
    public class CsvWriterStep : BaseStep, IPipelineStep
    {
        private bool _skipEmptyResults = true;

        public override RaceStepData Process(RaceStepData step)
        {
            if (_skipEmptyResults && step.RaceData.Results.Any())
            {
                var firstResult = step.RaceData.Results.First();

                /* if race name and date is not present in results/csv, get them from filename and set them for each result */
                if (firstResult.RaceDate == DateTime.MinValue && step.RaceData.Date.HasValue)
                {
                    step.RaceData.Results.ForEach(result => result.RaceDate = step.RaceData.Date.ValueOrDefault());
                }
                if (string.IsNullOrEmpty(firstResult.Race) && step.RaceData.Name.HasValue)
                {
                    step.RaceData.Results.ForEach(result => result.Race = step.RaceData.Name.ValueOrDefault());
                }
                var raceDate = firstResult.RaceDate;
                var raceName = firstResult.Race;

                var destPath = step.OutputFolder;
                var resultRows = step.RaceData.Results;
                var raceType = step.RaceData.RaceType;

                if(string.IsNullOrEmpty(raceName))
                {
                    throw new Exception("no racename: " + step.FullPath);
                }
                Write(destPath, raceDate, raceName, resultRows, raceType.ValueOrDefault());
            }

            return step;
        }

        protected void Write(string destFolder, DateTime raceDate, string raceName, IEnumerable<ResultRow> rows, string raceType)
        {
            foreach (var row in rows)
            {
                if (string.IsNullOrEmpty(row.Race))
                {
                    row.Race = raceName;
                }
            }

            var dir = new DirectoryInfo(destFolder);
            if (!Directory.Exists(dir.FullName))
            {
                Console.WriteLine("Creating dir: " + dir.FullName);
                Directory.CreateDirectory(dir.FullName);
            }

            var csvReaderConfig = new Configuration() { HeaderValidated = null, SanitizeForInjection = false, TrimOptions = TrimOptions.Trim };

            var filename = String.Format("{0}_{1}_{2}.csv", raceDate.ToString("yyyy-MM-dd"), raceType, raceName.Replace(" ", "_"));
            var destFile = Path.Combine(destFolder, FixDestFilename(filename));
            Console.WriteLine("destFile: " + destFile);

            using (TextWriter writer = new StreamWriter(destFile))
            {
                var csvWriter = new CsvWriter(writer);

                csvWriter.WriteRecords<ResultRow>(rows);
            }
        }

        protected string FixDestFilename(string filename)
        {
            return filename.Replace(" ", "_").Replace(":", "_").Replace("/", "_").Replace("\\", "");
        }

    }
}
