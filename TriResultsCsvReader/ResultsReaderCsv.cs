using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;

namespace TriResultsCsvReader
{
    public class ResultsReaderCsv
    {
        private IEnumerable<Column> _columnsConfig;
        private StandardizeResultsCsv _csvStandardizer;

        public ResultsReaderCsv() : this("column_config.xml")
        {
        }

        public ResultsReaderCsv(string configFilePath)
        {
            if(string.IsNullOrEmpty(configFilePath))
            {
                throw new BadConfigurationException("No config file given");
            }

            //var isLocalPath = new Uri(configFilePath).is;

            //if (isLocalPath)
            //{
            //    configFilePath = Path.Combine(Assembly.GetExecutingAssembly().Location, configFilePath);
            //}
            if(!File.Exists(configFilePath))
            {
                throw new BadConfigurationException($"Config file not found in path: {configFilePath}");
            }

            _columnsConfig = CsvConfigHelper.ReadConfig(configFilePath);
            _csvStandardizer = new StandardizeResultsCsv(_columnsConfig);
        }
        

        public IEnumerable<ResultRow> ReadFile(string csvFilename, Expression<Func<ResultRow, bool>> filter = null)
        {
            // 1. standardize header
            var csvLines = File.ReadAllLines(csvFilename);

            var standardizedCsv = _csvStandardizer.Read(csvLines);

            IEnumerable<ResultRow> records;

            // 2. read csv
            using (TextReader sr = new StringReader(String.Join("\n", standardizedCsv)))
            {
                var csvReaderConfig = new Configuration() { HeaderValidated = null, MissingFieldFound = null, SanitizeForInjection = true, TrimOptions = TrimOptions.Trim };
                var csvReader = new CsvReader(sr, csvReaderConfig);
                records = csvReader.GetRecords<ResultRow>().ToList();

                if (filter != null)
                {
                    var compiledFilter = filter.Compile();
                    records = records.Where(r => compiledFilter.Invoke(r));
                }

                records = records.ToList();
            }

            return records;
        }
    }
}
