using System;
using System.Collections.Generic;
using System.Linq;
using TriResultsCsvReader;

namespace TriResultsGeneratorBL
{
    public interface ICsvHelper
    {
        string[] ReplaceHeaders(string[] csvLines, Func<IEnumerable<string>, IEnumerable<string>> nameMapperFunc);
        char DetermineSeparator(string csvLine);
    }

    public class MyCsvHelper : ICsvHelper
    {
        public IEnumerable<string> GetHeaders(IEnumerable<string> csvLines)
        {
            var headerLine = csvLines.FirstOrDefault();
            if (string.IsNullOrEmpty(headerLine))
            {
                throw new InvalidCsvException("Csv has no headers");
            }

            var separator = DetermineSeparator(headerLine);

            return headerLine?.Split(separator).ToList() ?? new List<string>();
        }

        public string[] ReplaceHeaders(string[] csvLines, Func<IEnumerable<string>, IEnumerable<string>> nameMapperFunc)
        {
            char separator = ',';
            
            var lines = csvLines;
            if (lines.Any())
            {
                separator = DetermineSeparator(csvLines.First());
                var headers = lines.First();
                var standardizedHeaders = nameMapperFunc.Invoke(headers.Split(separator).ToList());
                lines[0] = String.Join(separator.ToString(), standardizedHeaders);
            }

            return lines;
        }

        public char DetermineSeparator(string csvHeader)
        {
            if (!string.IsNullOrEmpty(csvHeader))
            {
                var commaCount = csvHeader.Split(',').Count();
                var semiColonCount = csvHeader.Split(';').Count();

                return commaCount > semiColonCount ? ',' : ';';
            }

            return ',';
        }
    }
}
