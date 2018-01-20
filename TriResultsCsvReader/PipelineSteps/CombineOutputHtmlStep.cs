using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsCsvReader.PipelineSteps
{
    public class CombineOutputHtmlStep
    {
        public string Process(string destFolder, string outputFilename, IEnumerable<Column> columns, IEnumerable<Race> races)
        {
            var destFullPath = FileHelper.CreateDestFilePath(destFolder, DateTime.Now, outputFilename);

            var csvReaderConfig = new Configuration() { HeaderValidated = null, SanitizeForInjection = false, TrimOptions = TrimOptions.Trim };
            Console.WriteLine("destFile: " + destFullPath);


            var htmlBuilder = new StringBuilder();

            foreach (var race in races)
            {
                htmlBuilder.AppendLine(string.Format(@"<div class=""h3""><span class=""racename"">{0}</span><span class=""racedate"">{0}</span> </div>", race.Name, race.Date.ToString("dd-MMM-yyyy")));

                htmlBuilder.AppendLine("<table>");

                var showColumns = new Dictionary<string, bool>();

                htmlBuilder.AppendLine("<tr>");
                foreach (var column in columns) // headers
                {
                    //var properties = column.GetType().GetProperties();
                    //foreach (var prop in properties)
                    //{
                    var isNotEmptyColumn = race.Results.Any(r => null != r.GetPropertyValue(column.Name));
                    showColumns.Add(column.Name, isNotEmptyColumn);

                    if(isNotEmptyColumn) {
                        htmlBuilder.Append(string.Format("<th>{0}</th>", column.Name));
                    }
                }
                htmlBuilder.AppendLine("</tr>");
                var skippedColumns = new Dictionary<string,int>();
                Console.WriteLine("Race: " + race.Name);
                foreach (var result in race.Results)
                {
                    htmlBuilder.AppendLine("<tr>");
                    foreach (var column in columns)
                    {
                        if (showColumns[column.Name])
                        {
                            var columnValue = result.GetPropertyValue(column.Name);
                            htmlBuilder.Append(string.Format("<td>{0}</td>", columnValue));
                        }
                        else
                        {
                            if(!skippedColumns.ContainsKey(column.Name))
                            {
                                skippedColumns.Add(column.Name, 0);
                            }
                            skippedColumns[column.Name]++;
                            //Console.WriteLine("Skip column: {0}", column.Name);
                        }
                    }

                    htmlBuilder.AppendLine("</tr>");
                }

                htmlBuilder.AppendLine("</table>");

                htmlBuilder.Append("<p/>");

                var skippedColumnsStr = skippedColumns.Select(p => string.Join(", ", string.Format("{0}: {1} ", p.Key, p.Value)));
                Console.WriteLine(string.Format("Skip columns: {0}", String.Join(", ", skippedColumnsStr)));
            }

            return htmlBuilder.ToString();
        }
    }
}