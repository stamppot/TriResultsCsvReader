using System;
using System.Collections.Generic;
using System.Linq;
using ConfigReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TriResultsCsvReader.StandardizeHeaders;
using TriResultsGeneratorBL;

namespace TriResultsCsvReader.Test
{
    [TestClass]
    public class ColumnsTest
    {
        private IEnumerable<Column> _columnsConfig;

        public ColumnsTest()
        {
            IColumnConfigProvider columnConfigProvider = new ColumnConfigProvider("column_config.xml");
            _columnsConfig = columnConfigProvider.Get();
        }
        
        [TestMethod]
        public void SingleMappingColumnTest()
        {
            var csv = new List<string> { "Pos,StartNr,Naam,Club,Club/Woonplaats,Cat,PltsCat,Zwem,PltsZwem,Wiss1,Fiets,PltsFiets,NaFiets,PltsNaFie,Wiss2,Loop,PltsLoop,TotaalTijd,DQ" };
            var columnNames = new MyCsvHelper().GetHeaders(csv).ToList();

            var standardizedColumnNames = new ColumnStandardizer(_columnsConfig).GetStandardColumnNames(columnNames, _columnsConfig).ToList();

            Assert.AreEqual(columnNames.Count, standardizedColumnNames.Count);

            var expectedColumns = new List<string>
            {
                "Pos",
                "StartNr",
                "Naam",
                "Club",
                "City",
                "Cat",
                "Swim",
                "PosSwim",
                "T1",
                "Bike",
                "PosBike",
                "AfterBike",
                "PosAfterBike",
                "T2",
                "Run",
                "PosRun",
                "Total"
            };

            foreach (var expectedColumn in expectedColumns)
            {
                Assert.IsTrue(standardizedColumnNames.Contains(expectedColumn));
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
                Assert.IsFalse(standardizedColumnNames.Contains(unexpectedColumn));
            }
        }
    }
}
