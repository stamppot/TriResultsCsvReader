﻿using System;
using System.Collections.Generic;
using System.IO;
using TriResultsAppServices;

namespace FileAppServices
{
    public class CsvResultsReader : IRaceResultsReader
    {
        //private readonly Action<string> _logWriter;

        //public CsvResultsReader(Action<string> logWriter)
        //{
        //    _logWriter = logWriter == null ? new Action<string>((s) => Console.WriteLine(s)) : logWriter;
        //}

        public IEnumerable<string> ReadRaceRows(string filename)
        {

            // 1. standardize header
            var csvLines = File.ReadAllLines(filename);

            var columnValidator = new ValidateCsvNumberOfColumns();
            bool isValid = false;

            try
            {
                isValid = columnValidator.Validate(csvLines);
            }
            catch (FormatException ex)
            {
                var message = $"File doesn't have an equal amount of columns: {filename}.";
                throw new FormatException(message, ex);
            }

            if (!isValid)
            {
                Console.WriteLine("Error!  Number of columns mismatch in csv file: " + filename);
            }

            return csvLines;
        }
    }
}
