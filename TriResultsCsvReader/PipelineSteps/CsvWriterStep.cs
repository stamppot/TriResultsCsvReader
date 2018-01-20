using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsCsvReader
{
    public class CsvWriterStep : BaseStep, IPipelineStep
    {
        private bool _skipEmptyResults = true;

        public override StepData Process(StepData step)
        {
            if (_skipEmptyResults && step.RaceData.Results.Any())
            {
                var firstResult = step.RaceData.Results.First();
                var raceDate = firstResult.RaceDate;
                var raceName = firstResult.Race;

                var destPath = step.OutputFile;
                var resultRows = step.RaceData.Results;
                var raceType = step.RaceData.RaceType;

                Write(destPath, raceDate, raceName, resultRows, raceType);
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

            var filename = String.Format("{0}_{1}_{2}.csv", raceDate.ToString("yyyy-MM-dd"), raceType, raceName.Replace(" ", "_");
            var destFile = Path.Combine(destFolder, filename);
            Console.WriteLine("destFile: " + destFile);

            using (TextWriter writer = new StreamWriter(destFile))
            {
                var csvWriter = new CsvWriter(writer);

                csvWriter.WriteRecords<ResultRow>(rows);
            }
        }

    }
}
