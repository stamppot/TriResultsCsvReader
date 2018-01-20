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
    public class CsvOutputWriter
    {
        public void Write(string destFolder, DateTime raceDate, string raceName, IEnumerable<ResultRow> rows, string raceType)
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

            var filename = String.Format("{0}_{1}_{2}.csv", raceDate.ToString("yyyy-MM-dd"), raceType, raceName);
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
