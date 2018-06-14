using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TriResultsCsvReader;
using System.IO;
using ConfigReader;
using TriResultsCsvReader.StandardizeHeaders;

namespace ResultsCsvReader.Test
{
    [TestClass]
    public class ResultsReaderCsvTest
    {
        private ResultsReaderCsv _resultsReaderCsv;

        public ResultsReaderCsvTest()
        {
            IColumnConfigProvider columnConfigProvider = new ColumnConfigProvider("column_config.xml");
            _resultsReaderCsv = new ResultsReaderCsv(columnConfigProvider); 
        }

        [TestMethod]
        public void EqualColumnsTest()
        {
            var csvFile = "files/2017-aug-13-Nordseeman-2017.csv";

            var fullPath = Path.GetFullPath(csvFile);

            var dirExists = Directory.Exists(@"C:\Repos\CsvColumnNormalizer\ResultsCsvReader.Test\bin\Debug\files");
            if (File.Exists(fullPath)) {
                var validator = new ValidateCsvNumberOfColumns();
                var results = validator.Validate(csvFile);

                Assert.IsTrue(results);

            }
        }

        [TestMethod]
        public void ReadResults_LeiderdorpSprint_Test()
        {
            var csvFile = "files/Leiderdorp_sprint.csv";

            var results = _resultsReaderCsv.ReadFile(csvFile);

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void ReadResults_LeiderdorpIndi_Test()
        {
            var csvFile = "files/Leiderdorp_sprint_indi.csv";

            var results = _resultsReaderCsv.ReadFile(csvFile);

            Assert.IsTrue(results.Any());
        }
        
        [TestMethod]
        public void ReadResults_ouderkerkadamstel_sprint_m_Test()
        {
            var csvFile = "files/ouderkerkadamstel_sprint_m.csv";

            var results = _resultsReaderCsv.ReadFile(csvFile);

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void ReadResults_Rotterdam_RBR_overall_Test()
        {
            var csvFile = "files/Rotterdam_RBR_overall.csv";

            var results = _resultsReaderCsv.ReadFile(csvFile);

            Assert.IsTrue(results.Any());
        }
    }
}
