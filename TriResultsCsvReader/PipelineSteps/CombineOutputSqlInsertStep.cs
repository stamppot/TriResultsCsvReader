using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsCsvReader.PipelineSteps
{
    public class CombineOutputSqlInsertStep
    {
        private readonly string _tableName = "uitslagen";

        public string Process(string destFolder, string outputFilename, IEnumerable<Column> columns, IEnumerable<Race> races)
        {
            var destFullPath = FileHelper.CreateDestFilePath(destFolder, DateTime.Now, outputFilename, "sql");

            var csvReaderConfig = new Configuration() { HeaderValidated = null, SanitizeForInjection = false, TrimOptions = TrimOptions.Trim };
            Console.WriteLine("destFile: " + destFullPath);


            var htmlBuilder = new StringBuilder();

            foreach (var race in races)
            {
                //var raceDateNumeric = race.Date.ToString("dd-MM-yyyy");
                //if (race.Name.Contains(raceDateNumeric))
                //{
                //    race.Name = race.Name.Replace(raceDateNumeric, ""); // remove what will be a double occurrence of the date
                //}

                var showColumns = new Dictionary<string, bool>() { { "Race", true }, { "RaceDate", true } };

                var columnNames = new List<string>();
                foreach (var column in columns) // headers
                {
                    var isNotEmptyColumn = race.Results.Any(r => { var val = r.GetPropertyValue(column.Name); return ((val is String) && !string.IsNullOrEmpty((String)val) || (!(val is String) && val != null)); });
                    if (!showColumns.ContainsKey(column.Name))
                        showColumns[column.Name] = isNotEmptyColumn;

                    if (isNotEmptyColumn && showColumns[column.Name])
                    {
                        columnNames.Add(column.Name);
                    }
                }

                var columnsStr = "(" + string.Join(", ", columnNames) + ")";

                var skippedColumns = new Dictionary<string, int>();
                Console.WriteLine("Race: " + race.Name);

                var rowsValues = new List<string>();
                foreach (var result in race.Results)
                {
                    var values = new List<string>();
                    
                    foreach (var column in columns)
                    {
                        if (showColumns[column.Name])
                        {
                            var columnValue = result.GetPropertyValue(column.Name);
                            var columnType = result.GetPropertyType(column.Name);

                            string strFormat;
                            
                            if (columnType == PropertyType.anInt || columnType == PropertyType.aNullableInt)
                                strFormat = "{0}";
                            else if (columnType == PropertyType.aDate)
                            {
                                strFormat = "'{0}'"; // default for datetime and string
                                columnValue = ToDbDate((DateTime)columnValue);
                            }
                            else
                            {
                                strFormat = "'{0}'"; // default for datetime and string
                                columnValue = Sanitize(columnValue.ToString());
                            }

                            values.Add(string.Format(strFormat, columnValue));
                        }
                        else
                        {
                            if (!skippedColumns.ContainsKey(column.Name))
                            {
                                skippedColumns.Add(column.Name, 0);
                            }
                            skippedColumns[column.Name]++;
                            //Console.WriteLine("Skip column: {0}", column.Name);
                        }
                    }

                    rowsValues.Add(string.Format(" ({0})", string.Join(",", values)));
                }


                htmlBuilder.AppendLine(string.Format("insert into {0} {1} VALUES {2};", _tableName, columnsStr, string.Join(", ", rowsValues)));

                var skippedColumnsStr = skippedColumns.Select(p => string.Join(", ", string.Format("{0}: {1} ", p.Key, p.Value)));
                Console.WriteLine(string.Format("Skip columns: {0}", String.Join(", ", skippedColumnsStr)));
            }

            File.WriteAllText(destFullPath, htmlBuilder.ToString());
            return htmlBuilder.ToString();
        }

        private string Sanitize(string value)
        {
            return value.Replace("\'", "\'\'");
        }

        private string ToDbDate(DateTime value)
        {
            return value.ToString("yyyy-MM-dd");
        }
    }
}
