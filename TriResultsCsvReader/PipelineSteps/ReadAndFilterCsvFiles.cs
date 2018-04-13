using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TriResultsCsvReader.PipelineSteps
{
    public class ReadAndFilterCsvFiles
    {
        public List<RaceStepData> GetFilteredRaces(IEnumerable<string> inputFiles, string inputFolder, string outputFolder, DateTime raceDate, IEnumerable<Column> columnsConfig)
        {
            var filteredRaces = new List<RaceStepData>(inputFiles.Count());
            foreach (var file in inputFiles)
            {
                var filePath = Path.Combine(inputFolder, file);
                Console.WriteLine("filePAth! " + filePath);
                var srcFilename = raceDate;
                Console.Write("race (from filename): {0}, date: {1}, name: {2}", srcFilename, raceDate, file);


                var stepData = new RaceStepData { InputFile = filePath, ColumnConfig = columnsConfig, OutputOptions = new List<string> { "csv" } };

                var readAndFilterStep = new ReadFileAndStandardizeStep(columnsConfig);

                var nextStep = readAndFilterStep.Process(stepData);

                if(nextStep.RaceData != null)
                {
                    Console.WriteLine("Processed {0}", nextStep.RaceData.Name);
                }
                if (nextStep.RaceData != null)
                {
                    filteredRaces.Add(nextStep);
                }
                else
                {
                    Console.WriteLine("Cannot read race data from file '{0}'", file);
                }
            }
            
            return filteredRaces;
        }
    }
}
