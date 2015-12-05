using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockNewsScraper.Scraper
{
    public class ABC: NewsScraper
    {
        public ABC()
            : base("ABC")
        {
            articleList = new List<Links>();
        }

        protected override void GetLinks(string symbol)
        {
            string baseUrl = "http://search.abc.net.au/s/search.html?query={0}&collection=news_meta&form=simple";
            int currentNumberOfResult = 0;
            bool hasNext = false;

            Action<string> makeRequest =
                suffix =>
                {
                    string url = string.Format(baseUrl, symbol) + "&" + suffix;

                    string html = Utilities.MakeRequest(url);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    var list = doc.DocumentNode.SelectNodes("//ol[@id='fb-results']//h3//a");

                    foreach (var node in list)
                    {
                        string title = node.InnerText.Trim();
                        string articleUrl = node.GetAttributeValue("href", string.Empty);
                        articleList.Add(new Links { Title = title, Url = articleUrl });
                        currentNumberOfResult++;
                    }

                    hasNext = doc.DocumentNode.SelectSingleNode("//a[@class='next']") != null; 
                };

            makeRequest(string.Empty);
             
            while (currentNumberOfResult < maxNumberOfResults && hasNext)
            {
                makeRequest("start_rank=" + (currentNumberOfResult + 1));
            }
        }

        private int maxNumberOfResults = 500;
        private List<Links> articleList { get; set; }

        private class Links
        {
            public string Url { get; set; }
            public string Title { get; set; }
        }
    }
}

//http://search.abc.net.au/s/search.html?query=aapl&collection=abcall_meta&form=simple&start_rank=11