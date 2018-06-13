using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using HtmlAgilityPack;
using Optional;

namespace UrlResultsFetcher
{
    public class FetchUrlContents
    {

        public HtmlDocument GetPage(Uri uri)
        {
            var web = new HtmlWeb();
            var doc = web.Load(uri);

            return doc;
        }

        public Option<Tuple<string,DateTime>> GetRacename(HtmlDocument doc)
        {
            var allowedTdClass = new[] { "REPORTHEADER" };
            var containsClassFilter = new Func<string, bool>(className => allowedTdClass.Contains(className));

            var firstTable = GetResultsTables(doc).FirstOrDefault();
            var results = ParseAndFilterHtmlTable(firstTable, containsClassFilter, false).SelectMany(x => x).FirstOrDefault(x => x != null);

            var raceAndDate = DateUtils.FromRaceData(results);

            return raceAndDate;
        }

        public List<string> GetFieldNames(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//tr/td");
            return nodes.Where(node => node.GetClasses().Contains("FIELDNAMES")).Select(node => node.InnerText).ToList();
        }

        public DataSet GetResultsTable(HtmlDocument doc)
        {
            var tablesList = GetResultsLists(doc).ToList();

            var dataset = new DataSet("Results");

            for (int i = 0; i < tablesList.Count(); i++)
            {
                var tableRows = tablesList[i];
                var dt = new DataTable("Table " + i);
                dt.Clear();

                var fieldNames = tableRows.First().Where(f => !string.IsNullOrWhiteSpace(f)).ToList();
                tableRows.RemoveAt(0);

                foreach (var fieldName in fieldNames)
                {
                    if (!string.IsNullOrWhiteSpace(fieldName))
                        dt.Columns.Add(fieldName);
                }

                foreach (var row in tableRows)
                {
                    DataRow r = dt.NewRow();

                    var nameValueList = fieldNames.Zip(row, (field, cell) => new {Field = field, Value = cell});
                    foreach (var nameValue in nameValueList)
                    {
                        r[nameValue.Field] = nameValue.Value;
                    }

                    dt.Rows.Add(r);
                }

                dataset.Tables.Add(dt);
            }

            return dataset;
        }

        public IEnumerable<List<List<string>>> GetResultsLists(HtmlDocument doc)
        {
            var notAllowedTdClass = new[] {"colspan", "REPORTHEADER", "PAGEHEADER"};

            var notContainsClassFilter = new Func<string, bool>(className => !notAllowedTdClass.Contains(className));

            var tables = GetResultsTables(doc).Select(table => ParseAndFilterHtmlTable(table, notContainsClassFilter));

            var outputTables = new List<List<List<string>>>();
            foreach (var rowLists in tables)
            {
                if (rowLists.Any())
                {
                    var headers = rowLists.First();
                    var numColumns = headers.Count;

                    outputTables.Add(rowLists.Where(row => row.Count == numColumns).ToList());
                }
            }

            return outputTables;
        }

        protected IEnumerable<HtmlNode> GetResultsTables(HtmlDocument doc)
        {
            var tables = doc.DocumentNode.SelectNodes("//table").Cast<HtmlNode>();
            return tables;
        }

        protected List<List<string>> ParseAndFilterHtmlTable(HtmlNode tableNode, Func<string,bool> classFilter, bool onlySingleColspan = true)
        {
            var results = new List<List<string>>();

            foreach (var row in tableNode.SelectNodes("tr").Cast<HtmlNode>())
            {
                var rowList = new List<string>();
                foreach (var cell in row.SelectNodes("th|td"))
                {
                    if (onlySingleColspan)
                    {
                        var colspan = Int32.Parse(cell.GetAttributeValue("colspan", "1"));
                        if (colspan > 1) continue;
                    }

                    var className = cell.GetAttributeValue("class", "");
                    if (classFilter.Invoke(className))
                    {
                        var cellValue = cell.InnerText.Replace("&nbsp;", "").Replace("<br/>", "");
                        cellValue = cellValue.Replace("DNS", "").Replace("DNF", "");
                        rowList.Add(cellValue);
                    }
                }

                if (rowList.Any())
                {
                    results.Add(rowList);
                }
            }

            return results;
        }
    }
}
