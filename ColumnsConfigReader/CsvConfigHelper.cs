using System;
using System.Collections.Generic;
using System.IO;
using TriResultsCsvReader;

namespace ConfigReader
{
    public class CsvConfigHelper
    {
        public static IEnumerable<Column> ReadConfig(string configFile = "column_config.xml")
        {
            var csvColumnConfixXml = File.ReadAllText(configFile);
            var configReader = new ColumnsConfigReader();
            var columnsConfig = configReader.Read(csvColumnConfixXml);
            return columnsConfig;
        }
    }
}
