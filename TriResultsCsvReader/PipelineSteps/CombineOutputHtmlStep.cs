using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TriResultsCsvReader.PipelineSteps
{
    public class CombineOutputHtmlStep
    {
        public string Process(string destFolder, string outputFilename, IEnumerable<Column> columns, IEnumerable<Race> races)
        {
            var destFullPath = FileHelper.CreateDestFilePath(destFolder, DateTime.Now, outputFilename, "html");

            var csvReaderConfig = new Configuration() { HeaderValidated = null, SanitizeForInjection = false, TrimOptions = TrimOptions.Trim };
            Console.WriteLine("destFile: " + destFullPath);


            var htmlBuilder = new StringBuilder();

            foreach (var race in races)
            {
                var raceDateNumeric = race.Date.ToString("dd-MM-yyyy");
                if (race.Name.Contains(raceDateNumeric))
                {
                    race.Name = race.Name.Replace(raceDateNumeric, ""); // remove what will be a double occurrence of the date
                }
                htmlBuilder.AppendLine(string.Format(@"<div class=""h3 race""><span class=""racename"">{0}</span>  <span class=""racedistance"">{1}</span>  <span class=""racedate"">{2}</span> </div>", race.Name, race.Distance, race.Date.ToString("dd-MMM-yyyy")));

                htmlBuilder.AppendLine(@"<table class=""raceresults"">");

                var showColumns = new Dictionary<string, bool>() { { "Race", false }, { "RaceDate", false } };

                htmlBuilder.AppendLine("<tr>");
                foreach (var column in columns) // headers
                {
                    var isNotEmptyColumn = race.Results.Any(r => { var val = r.GetPropertyValue(column.Name); return ((val is String) && !string.IsNullOrEmpty((String)val) || (!(val is String) && val != null)); });
                    if(!showColumns.ContainsKey(column.Name))
                        showColumns[column.Name] = isNotEmptyColumn;

                    if(isNotEmptyColumn && showColumns[column.Name]) {
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
                            htmlBuilder.Append(string.Format(@"<td class=""{1}"">{0}</td>", columnValue, column.Name.ToLower()));
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

            File.WriteAllText(destFullPath, htmlBuilder.ToString());
            return htmlBuilder.ToString();
        }
    }
}