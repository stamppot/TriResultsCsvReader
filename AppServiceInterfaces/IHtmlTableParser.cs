using System;
using System.Collections.Generic;
using System.Data;
using HtmlAgilityPack;
using Optional;

namespace AppServiceInterfaces
{
    public interface IHtmlTableParser
    {
        string FieldNamesClass { get; }
        IEnumerable<string> RacenameClass { get; }
        IEnumerable<string> SkipHtmlClasses { get; }

        Option<Tuple<string, DateTime>> GetRacename(HtmlDocument doc);
        DataSet GetResultsTable(HtmlDocument doc);
        List<List<string>> ParseAndFilterHtmlTable(HtmlNode tableNode, Func<string, bool> classFilter, bool onlySingleColspan = true);
    }
}