using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TriResultsCsvReader.PipelineSteps
{
    public class CombineOutputCsv
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destFolder"></param>
        /// <param name="outputFilename"></param>
        /// <param name="races"></param>
        /// <returns>Dest filename</returns>
        public string Process(string destFolder, string outputFilename, IEnumerable<Race> races)
        {
            var destFullPath = FileHelper.CreateDestFilePath(destFolder, DateTime.Now, outputFilename);

            var csvReaderConfig = new Configuration() { HeaderValidated = null, SanitizeForInjection = false, TrimOptions = TrimOptions.Trim };
            Console.WriteLine("destFile: " + destFullPath);


            using (TextWriter writer = new StreamWriter(destFullPath))
            {
                var csvWriter = new CsvWriter(writer);

                foreach (var race in races)
                {
                    var results = race.Results.ToList();
                    results.Add(new ResultRow()); // add empty row divider
                    csvWriter.WriteRecords<ResultRow>(results);
                }
            }

            return destFullPath;
        }
    }
}
