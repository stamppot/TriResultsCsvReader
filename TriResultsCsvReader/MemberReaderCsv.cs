using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace TriResultsCsvReader
{
    public class MemberReaderCsv
    {
        public IEnumerable<MemberRow> Read(string csvFilename)
        {
            IEnumerable<MemberRow> records;

            // 2. read csv
            using (TextReader sr = new StreamReader(csvFilename))
            {
                var csvReaderConfig = new Configuration() { HeaderValidated = null, MissingFieldFound = null, SanitizeForInjection = true, TrimOptions = TrimOptions.Trim };
                var csvReader = new CsvReader(sr, csvReaderConfig);
                records = csvReader.GetRecords<MemberRow>().ToList();
            }

            return records;
        }

    }
}