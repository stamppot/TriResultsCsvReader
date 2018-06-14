using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Optional.Collections;
using TriResultsAppServices;
using TriResultsCsvReader;
using TriResultsDomainServices;

namespace TriResultsDomainServices
{
    // message endpoint (on boundary of application) receive data from outside, see functional architecture f# by Mark Seeman on Pluralsight

    public interface IInputMessageEndPoint
    {
        List<RaceEnvelope> GetFilteredRaces(IEnumerable<string> inputFiles, string inputFolder, string outputFolder,
            DateTime raceDate, IEnumerable<Column> columnsConfig);
    }

    public class ReadAndFilterCsvFiles : IInputMessageEndPoint
    {
        private readonly Action<string> _outputWriter;
        protected readonly List<string> InfoLogs;
        private readonly IRaceResultsReader _resultsReader;

        public ReadAndFilterCsvFiles(IRaceResultsReader resultsReader, List<string> infoLogs)
        {
            _outputWriter = (str => Console.WriteLine(str));
            InfoLogs = infoLogs;
            _resultsReader = resultsReader;
        }

        protected void WriteInfo(string message)
        {
            if (null != _outputWriter)
            {
                _outputWriter.Invoke(message + Environment.NewLine);
            }

            if (null != InfoLogs)
            {
                InfoLogs.Add(message + Environment.NewLine);
            }
        }

        public RaceEnvelope StandardizeRace(HeaderStandardizer headerStandardizer, RaceEnvelope raceEnvelope)
        {
            
                // 1. Read results, put these in raceEnvelope
                var lines = _resultsReader.ReadRaceRows(raceEnvelope.InputFile);


                // filtering happens here
                var resultRows = headerStandardizer.StandardizeHeaders(lines, raceEnvelope.InputFile).ToList();
                raceEnvelope.RaceData.Results = resultRows;

                var fillRaceDate = new GetRaceDataFromResults();


                raceEnvelope = fillRaceDate.FillRaceEnvelope(raceEnvelope, resultRows.FirstOrNone(r => r.Race != null));

                if (raceEnvelope.RaceData != null)
                {
                    Console.WriteLine("Processed {0}", raceEnvelope.RaceData.Name);
                }
                if (raceEnvelope.RaceData == null)
                {
                    Console.WriteLine("Cannot read race data from file '{0}'", raceEnvelope.InputFile);
                }

            return raceEnvelope;
        }

        public List<RaceEnvelope> GetFilteredRaces(IEnumerable<string> inputFiles, string inputFolder, string outputFolder, DateTime raceDate, IEnumerable<Column> columnsConfig)
        {
            var filteredRaces = new List<RaceEnvelope>(inputFiles.Count());
            foreach (var file in inputFiles)
            {
                var filePath = Path.Combine(inputFolder, file);
                Console.WriteLine("filePAth! " + filePath);
                var srcFilename = raceDate;
                Console.Write("race (from filename): {0}, date: {1}, name: {2}", srcFilename, raceDate, file);


                var stepData = new RaceEnvelope { InputFile = filePath, /*ColumnConfig = columnsConfig,*/ OutputOptions = new List<string> { "csv" } };


                // 1. Read results, put these in raceEnvelope
                var lines = _resultsReader.ReadRaceRows(file);


                // filtering happens here
                var columnStandardizer = new HeaderStandardizer(columnsConfig, WriteInfo);
                var resultRows = columnStandardizer.StandardizeHeaders(lines, file).ToList();
                stepData.RaceData.Results = resultRows;

                var fillRaceDate = new GetRaceDataFromResults();


                var nextStep = fillRaceDate.FillRaceEnvelope(stepData, resultRows.FirstOrNone(r => r.Race != null));

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
