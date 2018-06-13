
using AppServiceInterfaces;

namespace UrlResultsFetcher
{
    public class HtmlTableParserFactory
    {
        public static IHtmlTableParser Get(string url)
        {
            if(url.Contains("mylaps.com")) return new MyLapsHtmlTableParser();

            return new MyLapsHtmlTableParser();
        }
    }
}
