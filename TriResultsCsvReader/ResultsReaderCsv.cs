using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Read(string csv)
        {
            var csvHelper = new CsvHelper();
            var columnNames = csvHelper.GetHeaders(csv).ToList();

            var columnStandardizer = new ColumnStandardizer(GetColumnConfiguration);
            var standardizedColumnsNames = columnStandardizer.GetStandardColumnNames(columnNames);


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
