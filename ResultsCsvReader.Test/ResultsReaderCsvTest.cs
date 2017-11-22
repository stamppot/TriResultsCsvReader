using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TriResultsCsvReader;

namespace ResultsCsvReader.Test
{
    [TestClass]
    public class ResultsReaderCsvTest
    {
        private ResultsReaderCsv _resultsReaderCsv;

        public ResultsReaderCsvTest()
        {
            _resultsReaderCsv = new ResultsReaderCsv();
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
