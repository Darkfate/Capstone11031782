using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

                    if (list == null)
                    {
                        return;
                    }

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

        public override void Scrape(string symbol)
        {
            base.Scrape(symbol);

            foreach (var article in articleList)
            {
                try
                {
                    string response = Utilities.MakeRequest(article.Url);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(response);

                    HtmlNode node = doc.DocumentNode;

                    string body = node.SelectSingleNode("//div[@class='article section']").InnerText.Trim();

                    var dateMatch = Regex.Match(article.Url, @"\d\d\d\d\-\d\d-\d\d");

                    string dateString = dateMatch.Success ? dateMatch.Value : string.Empty;

                    DateTime date = DateTime.ParseExact(dateString, "yyyy-MM-dd", new CultureInfo("en-US"));
                    
                    Save(article.Title, body, date);

                }
                catch
                {
                }
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