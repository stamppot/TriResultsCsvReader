using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TriResultsCsvReader;
using System.Linq.Expressions;

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
            _columnsConfig = CsvConfigHelper.ReadConfig(_configFile);
            _csvStandardizer = new StandardizeResultsCsv(_columnsConfig);
        }

        [TestMethod]
        public void ReadCsvRotterdamLongTotTest()
        {
            var filename = "files/2016-jul-23-rotterdam-LongTot.csv";
            var csv = File.ReadAllLines(filename);

            var standardizedCsv = _csvStandardizer.Read(csv);
            var standardizedHeaders = new MyCsvHelper().GetHeaders(standardizedCsv).ToList();

            var expectedColumns = "Pos,StartNr,Name,Cat,Swim,T1,Bike,AfterBike,T2,Run,Total".Split(',').ToList();
            foreach (var expectedColumn in expectedColumns)
            {
                Assert.IsTrue(standardizedHeaders.Contains(expectedColumn));
            }

            var resultsReaderCsv = new ResultsReaderCsv(_configFile, (str => Console.WriteLine(str)));
            var resultsWriterCsv = new TriResultsCsvWriter(_configFile, (str => Console.WriteLine(str)));

            var members = new MemberReaderCsv().Read("leden2016.csv");
            var memberWhitelist = new WhitelistFilter(members.Select(m => m.Name));
            Expression<Func<ResultRow, bool>> filterExp = ((row) => memberWhitelist.ExactMatch(row.Naam));

            var destFile = "files/output_2016_jul_23-rotterdam-LongTot.csv";
            resultsWriterCsv.StandardizeCsv("rotterdamLong", DateTime.Now, filename, destFile, filterExp);

            //var unexpectedColumns = new List<string>()
            //{
            //    "Plts",
            //    "StartPlts",
            //    "PltsZwem",
            //    "Zwem",
            //    "Wis1",
            //    "Fiets",
            //    "Wiss2",
            //    "Wissel2",
            //    "Club/Woonplaats"
            //};

            //foreach (var unexpectedColumn in unexpectedColumns)
            //{
            //    Assert.IsFalse(standardizedHeaders.Contains(unexpectedColumn));
            //}


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
