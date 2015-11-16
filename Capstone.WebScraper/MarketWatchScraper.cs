using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Capstone.WebScraper
{
    public class MarketWatchScraper
    {
        public MarketWatchScraper()
        {
            saveLocation = System.Configuration.ConfigurationManager.AppSettings["DownloadPath"] + "Results/";
        }

        public void GetTop100MarketWatchNews(string companySymbol)
        {
            string baseUrl = "http://www.marketwatch.com/news/headline/getheadlines";

            string apiUrl = string.Format(
                "{0}?ticker={1}&countryCode={2}&dateTime={3}&docId={4}&docType={5}&sequence={6}&messageNumber={7}&count={8}&channelName={9}&topic={10}&_={11}",
                baseUrl,
                companySymbol,
                "US",
                WebUtility.UrlEncode(DateTime.Now.ToString("hh:mm \\a\\.\\m\\ MMM\\. d\\,\\ tttt")),
                string.Empty,
                "806",
                string.Empty,
                string.Empty,
                System.Configuration.ConfigurationManager.AppSettings["ArticleCount"],
                WebUtility.UrlEncode("/news/marketwatch/company/us/" + companySymbol.ToLower()),
                string.Empty,
                "1439898864326"
                );


            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);

                var response = request.GetResponse();

                string directory = saveLocation + companySymbol + "\\News\\";

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string json = string.Empty;

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    json = reader.ReadToEnd();
                }

                var a = JsonConvert.DeserializeObject<List<MarketWatchNews>>(json);


                string filePath = directory + "NewsList.txt";


                using (var writer = new StreamWriter(filePath))
                {
                    writer.Write(JsonConvert.SerializeObject(a));
                }


                Console.WriteLine(companySymbol + " Success");
            }
            catch (Exception e)
            {
            }

        }

        public void ScrapeMarketWatch(string companySymbol)
        {
            string directory = saveLocation + companySymbol + "\\News\\";

            List<MarketWatchNews> marketWatchNews;
            using (var fileReader = new StreamReader(directory + "NewsList.txt"))
            {
                marketWatchNews = JsonConvert.DeserializeObject<List<MarketWatchNews>>(fileReader.ReadLine());
            }

            foreach (var news in marketWatchNews)
            {
                string url = articleUrl + news.SeoHeadlineFragment;
                string html;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                var response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    html = reader.ReadToEnd();
                }

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                HtmlNode node = doc.DocumentNode;

                var bodyNode = node.SelectSingleNode("//div[@id='article-body']");
                if (bodyNode == null)
                {
                    bodyNode = node.SelectSingleNode("//div[@class='articlePage']");
                }

                if (bodyNode != null)
                {

                    var articleBody = bodyNode.InnerText;

                    string shrinkBody = Regex.Replace(articleBody, @"\s+", " ");

                    var dateMatch = Regex.Match(news.SeoHeadlineFragment, @"\d\d\d\d\-\d\d-\d\d");

                    string date = dateMatch.Success ? dateMatch.Value : string.Empty;

                    using (var writer = new StreamWriter(directory + date + " " + RemoveIlligalChar(news.HeadlineText)))
                    {
                        writer.Write(shrinkBody);
                    }
                }
            }

            using (var writer = new StreamWriter(saveLocation + "info.txt", true))
            {
                writer.WriteLine(companySymbol + "\t" + marketWatchNews.Count());
            }
        }

        class MarketWatchNews
        {
            public string HeadlineText { get; set; }
            public string SeoHeadlineFragment { get; set; }
            public string UniqueId { get; set; }
        }

        private string RemoveIlligalChar(string s)
        {
            string illegal = s;
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                illegal = illegal.Replace(c.ToString(), "");
            }

            return illegal;
        }

        private string saveLocation;
        private const string articleUrl = "http://www.marketwatch.com/story";
    }
}
