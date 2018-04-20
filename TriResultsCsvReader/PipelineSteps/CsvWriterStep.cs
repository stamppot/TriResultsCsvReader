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

        public override RaceEnvelope Process(RaceEnvelope raceStepData)
        {
            if (_skipEmptyResults && raceStepData.RaceData.Results.Any())
            {
                var firstResult = raceStepData.RaceData.Results.First();

                /* if race name and date is not present in results/csv, get them from filename and set them for each result */
                if (firstResult.RaceDate == DateTime.MinValue && raceStepData.RaceData.Date.HasValue)
                {
                    raceStepData.RaceData.Results.ForEach(result => result.RaceDate = raceStepData.RaceData.Date.ValueOrDefault());
                }
                if (string.IsNullOrEmpty(firstResult.Race) && raceStepData.RaceData.Name.HasValue)
                {
                    raceStepData.RaceData.Results.ForEach(result => result.Race = raceStepData.RaceData.Name.ValueOrDefault());
                }
                var raceDate = firstResult.RaceDate;
                var raceName = firstResult.Race;

                var destPath = raceStepData.OutputFolder;
                var resultRows = raceStepData.RaceData.Results;
                var raceType = raceStepData.RaceData.RaceType;

                if(string.IsNullOrEmpty(raceName))
                {
                    throw new Exception("no racename: " + raceStepData.FullPath);
                }
                Write(destPath, raceDate, raceName, resultRows, raceType.ValueOrDefault());
            }

            return raceStepData;
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
