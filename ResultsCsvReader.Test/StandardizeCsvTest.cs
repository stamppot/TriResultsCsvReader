using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TriResultsCsvReader;
using System.Linq.Expressions;
using ConfigReader;
using FileAppServices;
using TriResultsAppServices;
using TriResultsCsvReader.PipelineSteps;
using TriResultsCsvReader.StandardizeHeaders;
using TriResultsDomainServices;

namespace CsvColumnNormalizer.Test
{
    [TestClass]
    public class StandardizeCsvTest
    {
        private readonly string _configFile = "column_config.xml";
        private IEnumerable<Column> _columnsConfig;
        private StandardizeResultsCsv _csvStandardizer;

        public StandardizeCsvTest()
        {
            IColumnConfigProvider columnConfigProvider = new ColumnConfigProvider("column_config.xml");
            _columnsConfig = columnConfigProvider.Get().ToList();
            _csvStandardizer = new StandardizeResultsCsv(_columnsConfig);
        }

        public IEnumerable<string> ReadFileResults(string file)
        {
            return new CsvResultsReader().ReadRaceRows(file);
        }

        [TestMethod]
        public void TestMultipleFiles()
        {
            var inputFolder = "files";
            var inputFiles = new DirectoryInfo(inputFolder).GetFiles().Select(fi => fi.Name);
            var outputFolder = "output";
            var infoLogs = new List<string>();
            IRaceResultsReader csvReader = new CsvResultsReader();
            var readAndFilterFiles = new ReadAndFilterCsvFiles(csvReader, infoLogs);

            var filteredRaces = readAndFilterFiles.GetFilteredRaces(inputFiles, inputFolder, outputFolder, DateTime.Now, _columnsConfig);

            var htmlOutputWriter = new CombineOutputHtmlStep();
            var outputString = htmlOutputWriter.Process(outputFolder, "uitslagentest", _columnsConfig, filteredRaces.Select(x => x.RaceData));

            Assert.IsTrue(!string.IsNullOrEmpty(outputString));

            var outputFile = Path.Combine(inputFolder, outputFolder, string.Format("{0}_uitslagen_html.html", DateTime.Now.ToString("yyyyMMddhhmm")));
            Directory.CreateDirectory(Path.Combine(inputFolder, outputFolder));
            File.WriteAllText(outputFile, outputString);
        }

        [TestMethod]
        public void ReadCsvRotterdamLongTotTest()
        {
            var filename = "files/2016-jul-24-rotterdam-NKTot.csv";
            
            filename = "files/2017-sep-9-almere-UitslagHTTot.csv";
            var csv = File.ReadAllLines(filename);

            var standardizedCsv = _csvStandardizer.Read(csv);
            var standardizedHeaders = new MyCsvHelper().GetHeaders(standardizedCsv).ToList();

            //var expectedColumns = "Pos,StartNr,Naam,Cat,Swim,T1,Bike,AfterBike,T2,Run,Total".Split(',').ToList();
            var expectedColumns = "Pos,StartNr,Naam,Cat,Swim,Bike,AfterBike,Run,Total".Split(',').ToList();
            foreach (var expectedColumn in expectedColumns)
            {
                Assert.IsTrue(standardizedHeaders.Contains(expectedColumn));
            }

            var members = new MemberReaderCsv().Read("leden2016.csv");
            var memberWhitelist = new WhitelistFilter(members.Select(m => m.Name));
            Expression<Func<ResultRow, bool>> filterExp = ((row) => memberWhitelist.ExactMatch(row.Naam));
            var readAndFilterStep = new ReadFileAndStandardizeStep(_columnsConfig);
            var destFile = "files/output";
            var srcFile = "files/2017-sep-9-almere-UitslagHTTot.csv";
            var stepData = new RaceEnvelope { InputFile = srcFile, FullPath = filename, OutputFolder = destFile };
            
            var race = readAndFilterStep.Process(stepData);

            var firstResult = race.RaceData.Results.First();
            Assert.AreEqual(4, race.RaceData.Results.Count());
            Assert.IsTrue(!string.IsNullOrEmpty(firstResult.Race));
            var writeStep = new CsvWriterStep().Process(race);
          
        }

        [TestMethod]
        public void ReadCsvAndStandardizeHeadersTest()
        {
            var csv = File.ReadAllLines("files/Leiderdorp_sprint.csv");

            var standardizedCsv = _csvStandardizer.Read(csv);
            var standardizedHeaders = new MyCsvHelper().GetHeaders(standardizedCsv).ToList();

            var expectedColumns = "Pos,StartNr,Naam,Cat,Swim,T1,Bike,AfterBike,T2,Run,Total".Split(',').ToList();
                        foreach (var expectedColumn in expectedColumns)
            {
                Assert.IsTrue(standardizedHeaders.Contains(expectedColumn));
            }

            var unexpectedColumns = new List<string>()
            {
                "Plts",
                "StartPlts",
                "PltsZwem",
                "Zwem",
                "Wis1",
                "Fiets",
                "Wiss2",
                "Wissel2",
                "Club/Woonplaats"
            };

            foreach (var unexpectedColumn in unexpectedColumns)
            {
                Assert.IsFalse(standardizedHeaders.Contains(unexpectedColumn));
            }


        }
    }
}
