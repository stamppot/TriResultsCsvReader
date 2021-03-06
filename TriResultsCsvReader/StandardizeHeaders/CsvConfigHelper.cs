﻿using System;
using System.Collections.Generic;
using System.IO;

namespace TriResultsCsvReader
{
    public class CsvConfigHelper
    {
        public IEnumerable<Column> ReadConfig(string configFile = "column_config.xml")
        {
            var csvColumnConfixXml = File.ReadAllText(configFile);
            var configReader = new ColumnsConfigReader();
            var columnsConfig = configReader.Read(csvColumnConfixXml);
            return columnsConfig;
        }
    }
}
