﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using CsvHelper;
using CsvHelper.Configuration;

namespace TriResultsCsvReader
{
    public class ResultsReaderCsv
    {
        private readonly Action<string> _outputWriter;
        private IEnumerable<Column> _columnsConfig;
        private StandardizeResultsCsv _csvStandardizer;

        public ResultsReaderCsv(Action<string> outputWriter = null) : this("column_config.xml", outputWriter)
        {
            _outputWriter = outputWriter;
        }

        public ResultsReaderCsv(string configFilePath, Action<string> outputWriter)
        {
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

            _columnsConfig = CsvConfigHelper.ReadConfig(configFilePath);
            _csvStandardizer = new StandardizeResultsCsv(_columnsConfig);
        }

        public ResultsReaderCsv(IEnumerable<Column> config, Action<string> outputWriter) {
            _columnsConfig = config;
            _outputWriter = outputWriter;
            _csvStandardizer = new StandardizeResultsCsv(_columnsConfig);

        }




        public IEnumerable<ResultRow> ReadFile(string csvFilename, Expression<Func<ResultRow, bool>> filter = null)
        {
            // 1. standardize header
            var csvLines = File.ReadAllLines(csvFilename);

            var columnValidator = new ValidateCsvNumberOfColumns();
            bool isValid = false;

            try
            {
                isValid = columnValidator.Validate(csvLines);
            }
            catch (FormatException ex)
            {
                var message = $"File doesn't have an equal amount of columns: {csvFilename}.";
                throw new CsvFormatException(message, ex);
            }

            if(!isValid)
            {
                Console.WriteLine("Error!  Number of columns mismatch in csv file: " + csvFilename);
            }

            var standardizedCsv = _csvStandardizer.Read(csvLines);

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
                    var message = string.Format("Error parsing csv file (Check column types and missing data and/or DNF where position should be: {0}: {1}", csvFilename, ex.Message);
                    throw new CsvFormatException(message, ex);
                } catch(Exception ex)
                {
                    Console.WriteLine("Error reading csv file: {0}: {1}", csvFilename, ex.Message);
                    
                    throw;
                }
                if (filter != null)
                {
                    var compiledFilter = filter.Compile();
                    records = records.Where(r => compiledFilter.Invoke(r));
                }

                records = records.ToList();

                WriteOutput($"Records read from file {csvFilename}: {records.Count()}");
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
