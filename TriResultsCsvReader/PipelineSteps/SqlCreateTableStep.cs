using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsCsvReader.PipelineSteps
{
    public class SqlCreateTableStep
    {
        private readonly string _tableName = "uitslagen";

        public string Process(/*string destFolder, string outputFilename,*/ IEnumerable<Column> columns)
        {
            //var destFullPath = FileHelper.CreateDestFilePath(destFolder, DateTime.Now, outputFilename, "sql");

            //var csvReaderConfig = new Configuration() { HeaderValidated = null, SanitizeForInjection = false, TrimOptions = TrimOptions.Trim };
            //Console.WriteLine("destFile: " + destFullPath);

            var result = new ResultRow();

            var dbColumns = columns.Select(column =>
            {
                var columnType = result.GetPropertyType(column.Name);

                var dbType = "varchar(255) null";
                if (columnType == PropertyType.anInt)
                {
                    dbType = "int(11)";
                }

                if (columnType == PropertyType.aNullableInt)
                {
                    dbType = "int(11) null";
                }

                if (columnType == PropertyType.aDate)
                {
                    dbType = "Date"; // nb. no time component!
                }

                return new { Name = column.Name, DbType = dbType };
            });


            var dbTypesStr = dbColumns.Select(c => string.Format("{0} {1}", c.Name, c.DbType));

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine(string.Format("CREATE TABLE {0} ({1});", _tableName, string.Join(",\n", dbTypesStr)));

            return strBuilder.ToString();
        }
    }
}