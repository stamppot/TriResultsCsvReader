using System;
using System.Data;
using AppServiceInterfaces;
using HtmlAgilityPack;
using Optional;

namespace UrlResultsFetcher
{
    public class ResultsPageFetcher : IHtmlResultsFetch
    {
        private HtmlDocument _doc;
        private IHtmlTableParser _htmlTableParser;
        private Uri _uri;
        
        public Option<Tuple<String,DateTime>> Info { get; set; }

        public ResultsPageFetcher(IHtmlTableParser htmlTableParser, Uri uri)
        {
            _htmlTableParser = htmlTableParser;
            _uri = uri;
        }

        public HtmlDocument GetHtmlDocument()
        {
            if (null == _doc)
            {
                var web = new HtmlWeb();
                _doc = web.Load(_uri);
            }

            return _doc;
        }


        public Option<Tuple<string, DateTime>> GetRaceData()
        {
            _doc = GetHtmlDocument();

            if (!Info.HasValue)
            {
                Info = _htmlTableParser.GetRacename(_doc);
            }

            return Info;
        }

        public DataSet GetData()
        {
             return _htmlTableParser.GetResultsTable(_doc);
        }
    }
}
