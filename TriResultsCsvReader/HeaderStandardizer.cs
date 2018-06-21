using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using TriResultsCsvReader.StandardizeHeaders;

namespace TriResultsCsvReader
{
    public class HeaderStandardizer
    {
        private readonly Action<string> _outputWriter;
        private IEnumerable<Column> _columnsConfig;
        private StandardizeResultsCsv _csvStandardizer;
        private IColumnConfigProvider _configProvider;

        public HeaderStandardizer(IColumnConfigProvider configProvider, Action<string> outputWriter = null) : this("column_config.xml", outputWriter)
        {
            _configProvider = configProvider;
        }

        public HeaderStandardizer(string configFilePath, Action<string> outputWriter)
        {
            if (null != outputWriter) _outputWriter = outputWriter;

            if(string.IsNullOrEmpty(configFilePath))
            {
                throw new BadConfigurationException("No config file given");
            }

            if(!File.Exists(configFilePath))
            {
                var errorMessage = $"Config file not found in path: {configFilePath}";
                _outputWriter.Invoke(errorMessage);
                throw new BadConfigurationException(errorMessage);
            }

            _columnsConfig = _configProvider.Get();
            _csvStandardizer = new StandardizeResultsCsv(_columnsConfig);
        }

        public HeaderStandardizer(IEnumerable<Column> config, Action<string> outputWriter) {
            _columnsConfig = config;
            _outputWriter = outputWriter;
            _csvStandardizer = new StandardizeResultsCsv(_columnsConfig);
        }


        public IEnumerable<ResultRow> StandardizeHeaders(IEnumerable<string> csvLines, string debugFilename)
        {
            var lines = csvLines.ToList();
            if(lines.Any() && new Regex(@"^\d").IsMatch(lines.First()))
            {
                throw new FormatException($"No header found in file {debugFilename}: " + lines.First());
            }
            
            // 1. standardize header
            var standardizedCsv = _csvStandardizer.Read(lines.ToArray());

            IEnumerable<ResultRow> records;

            // 2. read csv
            var csvStr = String.Join("\n", standardizedCsv);
            using (TextReader sr = new StringReader(csvStr))
            {
                var csvReaderConfig = new Configuration() { HeaderValidated = null, MissingFieldFound = null, SanitizeForInjection = true, TrimOptions = TrimOptions.Trim };
                var csvReader = new CsvReader(sr, csvReaderConfig);
                try
                {
                    records = csvReader.GetRecords<ResultRow>().ToList();
                }
                catch (CsvHelperException ex)
                {
                    var message = string.Format("Error parsing file (Check column types and missing data and/or DNF where position should be: {0}: {1}", debugFilename, ex.Message);
                    throw new FormatException(message, ex);
                } catch(Exception ex)
                {
                    Console.WriteLine("Error reading file: {0}: {1}", debugFilename, ex.Message);
                    
                    throw;
                }

                records = records.ToList();

                WriteOutput($"Records read from file {debugFilename}: {records.Count()}");
            }

            return records;
        }

        private void WriteOutput(string message)
        {
            if (null != _outputWriter)
            {
                _outputWriter.Invoke(message);
            }
        }
    }
}
