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

            var results = GetResultsLists(doc, containsClassFilter, false).SelectMany(x => x).FirstOrDefault(x => x != null);

            var raceAndDate = DateUtils.FromRaceData(results);

            return raceAndDate;
        }

        public List<string> GetFieldNames(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//tr/td");
            return nodes.Where(node => node.GetClasses().Contains("FIELDNAMES")).Select(node => node.InnerText).ToList();
        }

        public DataTable GetResultsTable(HtmlDocument doc)
        {
            var results = GetResultsLists(doc);

            var dt = new DataTable("Results");
            dt.Clear();

            var fieldNames = results.First();
            results.RemoveAt(0);

            foreach (var fieldName in fieldNames)
            {
                dt.Columns.Add(fieldName);
            }

            foreach (var row in results)
            {
                DataRow r = dt.NewRow();

                var nameValueList = fieldNames.Zip(row, (field, cell) => new { Field = field, Value = cell });
                foreach (var nameValue in nameValueList)
                {
                    r[nameValue.Field] = nameValue.Value;
                }

                dt.Rows.Add(r);
            }

            return dt;
        }

        public List<List<string>> GetResultsLists(HtmlDocument doc)
        {
            var notAllowedTdClass = new[] {"colspan", "REPORTHEADER", "PAGEHEADER"};

            var notContainsClassFilter = new Func<string, bool>(className => !notAllowedTdClass.Contains(className));

            return GetResultsLists(doc, notContainsClassFilter);
        }

        protected List<List<string>> GetResultsLists(HtmlDocument doc, Func<string,bool> classFilter, bool onlySingleColspan = true)
        {
            var table = doc.DocumentNode.SelectNodes("//table").Cast<HtmlNode>().FirstOrDefault();
            if (table == null) return null;

            var results = new List<List<string>>();

            foreach (var row in table.SelectNodes("tr").Cast<HtmlNode>())
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
