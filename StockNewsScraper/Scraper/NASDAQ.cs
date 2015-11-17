using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockNewsScraper.Scraper
{
    public class NASDAQ : NewsScraper
    {
        public NASDAQ()
            : base("NASDAQ")
        {
            articleList = new List<NasdaqLink>();
        }

        protected override void GetLinks(string symbol)
        {
            try
            {
                // Url for making request
                string url = "http://www.nasdaq.com/symbol/" + symbol + "/news-headlines";
                bool hasNextPage = false;
                string nextPageUrl = string.Empty;
                // Action to collect the link on each page
                Action<string> scrapeLinks =
                    html =>
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(html);

                        HtmlNode node = doc.DocumentNode;

                        var headlines = node.SelectSingleNode("//div[@class='headlines']");

                        foreach (var headline in headlines.SelectNodes("//a[@target='_self']"))
                        {
                            if (headline.InnerText.Trim() != string.Empty)
                            {
                                articleList.Add(
                                    new NasdaqLink
                                    {
                                        Title = headline.InnerText.Trim(),
                                        Url = headline.GetAttributeValue("href", string.Empty)
                                    });
                            }
                        }
                        // Check if next button ecist on the page
                        var pageNode = node.SelectSingleNode("//ul[@class='pager']");
                        hasNextPage = pageNode != null;
                        // Move to next page
                        if(hasNextPage)
                        {
                            var nextNode = pageNode.ChildNodes.First(n => n.InnerText.Trim().Contains("next"));

                            if (nextNode != null && nextNode.SelectSingleNode("a[@class='pagerlink']") != null)
                            {
                                nextPageUrl = nextNode.SelectSingleNode("a[@class='pagerlink']").GetAttributeValue("href", string.Empty);
                            }
                            else
                            {
                                hasNextPage = false;
                            }
                        }

                    };

                string response = Utilities.MakeRequest(url);

                scrapeLinks(response);
                // Continuously scrape until no pages are left
                while (hasNextPage)
                {
                    scrapeLinks(Utilities.MakeRequest(nextPageUrl));
                }
            }
            catch
            {
            }
        }

        public override void Scrape(string symbol)
        {
            base.Scrape(symbol);
            // Collect data from each ;oml
            foreach (var article in articleList)
            {
                try
                {
                    string response = Utilities.MakeRequest(article.Url);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(response);

                    HtmlNode node = doc.DocumentNode;

                    string body = node.SelectSingleNode("//div[@id='articleText']").InnerText.Trim();

                    HtmlNode dateNode = node.SelectSingleNode("//span[@itemprop='datePublished']");

                    DateTime date;

                    if(!DateTime.TryParseExact(dateNode.InnerText.Trim(), "MMM dd, yyyy hh:tt:ss TT", new CultureInfo("en-US"), DateTimeStyles.AllowInnerWhite, out date))
                    {
                        date = DateTime.ParseExact(dateNode.GetAttributeValue("content", string.Empty), "yyyy-MM-dd", new CultureInfo("en-US"));
                    }

                    Save(article.Title, body, date);

                }
                catch
                {

                }
            }

            articleList = new List<NasdaqLink>();

        }

        private List<NasdaqLink> articleList;

        class NasdaqLink
        {
            public string Title { get; set; }
            public string Url { get; set; }
        }
    }
}
