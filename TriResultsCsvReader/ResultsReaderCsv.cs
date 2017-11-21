using System;
using System.Collections.Generic;

namespace TriResultsCsvReader
{
    public class ResultsReaderCsv
    {
        private IEnumerable<Column> _csvColumnConfig;
        private readonly string _csvColumnConfixXml;

        public ResultsReaderCsv(string columnConfigXml)
        {
            _csvColumnConfixXml = columnConfigXml;
        }

        public ResultsReaderCsv(IEnumerable<Column> columnsConfig)
        {
            _csvColumnConfig = columnsConfig;
        }

        /// <summary>
        /// Reads csv file and standardizes columnnames
        /// </summary>
        /// <param name="csv"></param>
        public string[] Read(string[] csvLines)
        {
            ICsvHelper csvHelper = new MyCsvHelper();
            var columnStandardizer = new ColumnStandardizer(GetColumnConfiguration);
            
            var standardizedCsv = csvHelper.ReplaceHeaders(csvLines, columnStandardizer.GetStandardColumnNames);
            return standardizedCsv;
        }


        protected IEnumerable<Column> GetColumnConfiguration
        {
            get
            {
                if (null == _csvColumnConfig)
                {
                    var configReader = new ColumnsConfigReader();
                    _csvColumnConfig = configReader.Read(_csvColumnConfixXml);
                }
                return _csvColumnConfig;
            }
        }
    }
}
