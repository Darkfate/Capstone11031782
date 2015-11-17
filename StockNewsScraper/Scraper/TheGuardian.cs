using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockNewsScraper.Scraper
{
    public class TheGuardian: NewsScraper
    {
        public TheGuardian()
            : base("The Guardian")
        {
            resultLinks = new List<GoogleJsonResponse.Result>();
        }

        protected override void GetLinks(string symbol)
        {
            int count = 0;

            // Function to build url
            Func<string> getGoogleUrl = 
                () => string.Format(
                        "https://www.googleapis.com/customsearch/v1element?key=AIzaSyCVAXiUzRYsML1Pv6RwSG1gunmMikTzQqY&num=10&hl=en&source=gcsc&cx=007466294097402385199%3Am2ealvuxh1i&q={0}&sort=date%3Ar%3A{1}%3A{2}&googlehost=www.gogole.com&start={3}",
                        symbol, StartDate, EndDate, count);

            string url = getGoogleUrl();

            string json = Utilities.MakeRequest(url);

            var googleResponse = JsonConvert.DeserializeObject<GoogleJsonResponse>(json);

            int totalResult = googleResponse.cursor.estimatedResultCount;
            resultLinks.AddRange(googleResponse.results);

            count = count + googleResponse.results.Count();

            // Move to next page and scrape again. For somereason we can't get more than 90 results
            while (count < totalResult && count < 90)
            {
                json = Utilities.MakeRequest(getGoogleUrl());

                // Deserialise JSON response and add it to the list.
                googleResponse = JsonConvert.DeserializeObject<GoogleJsonResponse>(json);

                resultLinks.AddRange(googleResponse.results);

                count = count + googleResponse.results.Count();
            }

        }

        public override void Scrape(string symbol)
        {
            base.Scrape(symbol);

            // Loop through each result and collect data
            foreach (var result in resultLinks)
            {
                try
                {
                    if (result.url.Contains("/business/"))
                    {
                        string response = Utilities.MakeRequest(result.url);

                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(response);

                        HtmlNode node = doc.DocumentNode;

                        string title = node.SelectSingleNode("//h1[@itemprop='headline']").InnerText.Trim();
                        string body = node.SelectSingleNode("//div[@itemprop='articleBody']").InnerText.Trim();
                        string dateString = node.SelectSingleNode("//time[@itemprop='datePublished']").GetAttributeValue("datetime", string.Empty);
                        DateTime date = DateTime.Parse(dateString);

                        Save(title, body, date);
                    }
                }
                catch
                {
                }
            }

            resultLinks = new List<GoogleJsonResponse.Result>();

        }

        private List<GoogleJsonResponse.Result> resultLinks;
        private const string articleUrl = "http://www.marketwatch.com/story";

        // Class for JSON response
        private class GoogleJsonResponse
        {
                public Context context { get; set; }
                public Cursor cursor { get; set; }
                public List<Result> results { get; set; }

                public class Context
                {
                    public string title { get; set; }
                    public int total_results { get; set; }
                }

                public class Cursor
                {
                    public int currentPageIndex { get; set; }
                    public int estimatedResultCount { get; set; }
                    public bool isExactToTotalResult { get; set; }
                    public List<Page> pages { get; set; }
                    public string resultCount { get; set; }
                    public double searchResultTime { get; set; }

                    public class Page
                    {
                        public int label { get; set; }
                        public string start { get; set; }
                    }
                }

                public class Result
                {
                    public string cacheUrl { get; set; }
                    public string content { get; set; }
                    public string formattedUrl { get; set; }
                    public string unescapedUrl { get; set; }
                    public string url { get; set; }
                }
        }
    }
}
