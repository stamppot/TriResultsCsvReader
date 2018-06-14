using System;
using System.Collections.Generic;
using TriResultsCsvReader;
using TriResultsCsvReader.StandardizeHeaders;

namespace ConfigReader
{
    public class ColumnConfigProvider : IColumnConfigProvider
    {
        private readonly string _configFilePath;

        public ColumnConfigProvider(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        public IEnumerable<Column> Get()
        {
            return CsvConfigHelper.ReadConfig(_configFilePath);
        }
    }
}
