
using AppServiceInterfaces;

namespace UrlResultsFetcher
{
    public class HtmlTableParserFactory
    {
        public static IHtmlTableParser Get(string url)
        {
            if(url.Contains("mylaps.com")) return new MyLapsHtmlTableParser();

            if (url.Contains("uitslagensoftware.nl")) return new MyLapsHtmlTableParser();

            return new MyLapsHtmlTableParser();
        }
    }
}
