using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockNewsScraper.Scraper
{
    public class MarketWatch : NewsScraper
    {
        public MarketWatch()
            : base("Market Watch")
        {
            saveLocation = System.Configuration.ConfigurationManager.AppSettings["DownloadPath"] + "Results/";
        }

        public override void Scrape(string symbol)
        {
            base.Scrape(symbol);

            foreach (var result in newsList)
            {
                try
                {
                    string response = Utilities.MakeRequest(articleUrl + result.SeoHeadlineFragment);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(response);

                    HtmlNode node = doc.DocumentNode;

                    var bodyNode = node.SelectSingleNode("//div[@id='article-body']");
                    if (bodyNode == null)
                    {
                        bodyNode = node.SelectSingleNode("//div[@class='articlePage']");
                    }

                    string body = bodyNode.InnerText.Trim();
                    string title = result.HeadlineText.Trim();

                    var dateMatch = Regex.Match(result.SeoHeadlineFragment, @"\d\d\d\d\-\d\d-\d\d");

                    string dateString = dateMatch.Success ? dateMatch.Value : string.Empty;

                    DateTime date = DateTime.ParseExact(dateString, "yyyy-MM-dd", new CultureInfo("en-US"));

                    Save(title, body, date);

                    System.Threading.Thread.Sleep(1500);
                }
                catch
                {
                }
            }
        }

        protected override void GetLinks(string symbol)
        {
            string baseUrl = "http://www.marketwatch.com/news/headline/getheadlines";

            string apiUrl = string.Format(
                "{0}?ticker={1}&countryCode={2}&dateTime={3}&docId={4}&docType={5}&sequence={6}&messageNumber={7}&count={8}&channelName={9}&topic={10}&_={11}",
                baseUrl,
                symbol,
                "US",
                WebUtility.UrlEncode(StartDate.ToString("hh:mm \\a\\.\\m\\ MMM\\. d\\,\\ tttt")),
                string.Empty,
                "806",
                string.Empty,
                string.Empty,
                System.Configuration.ConfigurationManager.AppSettings["ArticleCount"],
                WebUtility.UrlEncode("/news/marketwatch/company/us/" + symbol.ToLower()),
                string.Empty,
                "1439898864326"
                );


            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);

                var response = request.GetResponse();


                string json = string.Empty;

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    json = reader.ReadToEnd();
                }

                newsList = JsonConvert.DeserializeObject<List<MarketWatchNews>>(json);

            }
            catch (Exception e)
            {
            }

        }

        class MarketWatchNews
        {
            public string HeadlineText { get; set; }
            public string SeoHeadlineFragment { get; set; }
            public string UniqueId { get; set; }
        }

        private string saveLocation;
        private const string articleUrl = "http://www.marketwatch.com/story";
        private List<MarketWatchNews> newsList { get; set; }
    }
}
