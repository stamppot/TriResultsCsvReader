using System;
using System.Collections.Generic;
using System.Linq;

namespace TriResultsCsvReader
{
    public interface ICsvHelper
    {
        string ReplaceHeaders(string csv, Func<IEnumerable<string>, IEnumerable<string>> nameMapperFunc);
        char DetermineSeparator(string csv);
    }

    public class CsvHelper : ICsvHelper
    {
        public IEnumerable<string> GetHeaders(string csv)
        {
            var headerLine = GetLines(csv).FirstOrDefault();
            if (string.IsNullOrEmpty(headerLine))
            {
                throw new InvalidCsvException("Csv has no headers");
            }

            var separator = DetermineSeparator(headerLine);

            return headerLine?.Split(separator).ToList() ?? new List<string>();
        }

        public string ReplaceHeaders(string csv, Func<IEnumerable<string>, IEnumerable<string>> nameMapperFunc)
        {
            char separator = ',';
            var lines = GetLines(csv);
            if (lines.Any())
            {
                separator = DetermineSeparator(csv);
                var headers = lines.First();
                var standardizedHeaders = nameMapperFunc.Invoke(headers.Split(separator).ToList());
                lines[0] = String.Join(separator.ToString(), standardizedHeaders);
            }

            return String.Join(separator.ToString(), lines);
        }

        public char DetermineSeparator(string csv)
        {
            var header = csv.Contains('\n') ? GetLines(csv).FirstOrDefault() : csv;
            
            if (header != null)
            {
                var commaCount = header.Split(',').Count();
                var semiColonCount = header.Split(';').Count();

                return commaCount > semiColonCount ? ',' : ';';
            }

            return ',';
        }

        private List<string> GetLines(string csv)
        {
            if (string.IsNullOrEmpty(csv))
            {
                throw new InvalidCsvException("Csv is empty");
            }

            return csv.Split('\n').ToList();
        }
    }
}
