using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsCsvReader
{
    public class FileHelper
    {
        public static string CreateDestFilePath(string destFolder, DateTime raceDate, string outputFilename, string extension = "csv")
        {
            var dir = new DirectoryInfo(destFolder);
            if (!Directory.Exists(dir.FullName))
            {
                Console.WriteLine("Creating dir: " + dir.FullName);
                Directory.CreateDirectory(dir.FullName);
            }

            var csvReaderConfig = new Configuration() { HeaderValidated = null, SanitizeForInjection = false, TrimOptions = TrimOptions.Trim };

            var filename = String.Format("{0}_{1}.{2}", raceDate.ToString("yyyy-MM-dd"), outputFilename, extension);
            var destFile = Path.Combine(destFolder, filename);

            return destFile;
        }
    }
}
