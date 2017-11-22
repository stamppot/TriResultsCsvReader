using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TriResultsCsvReader;

namespace CsvColumnNormalizer.Test
{
    [TestClass]
    public class StandardizeCsvTest
    {
        private IEnumerable<Column> _columnsConfig;
        private StandardizeResultsCsv _csvStandardizer;

        public StandardizeCsvTest()
        {
            _columnsConfig = CsvConfigHelper.ReadConfig("column_config.xml");
            _csvStandardizer = new StandardizeResultsCsv(_columnsConfig);
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
