using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriResultsCsvReader;

namespace ResultsCsvReader.Test
{
    [TestClass]
    public class ValidateCsvTest
    {
        [TestMethod]
        public void EqualColumnsTest()
        {
            var csvFile = "files/2017-aug-13-Nordseeman-2017.csv";

            var validator = new ValidateCsvNumberOfColumns();
            var results = validator.Validate(csvFile);

            Assert.IsTrue(results);
        }
    }
}