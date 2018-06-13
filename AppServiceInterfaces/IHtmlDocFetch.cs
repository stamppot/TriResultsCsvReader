using System;
using System.Data;
using HtmlAgilityPack;
using Optional;

namespace AppServiceInterfaces
{
    public interface IHtmlResultsFetch
    {
        HtmlDocument GetHtmlDocument();
        Option<Tuple<string, DateTime>> GetRaceData();
        DataSet GetData();
    }
}