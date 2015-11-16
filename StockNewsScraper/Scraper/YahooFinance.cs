using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockNewsScraper.Scraper
{
    public class YahooFinance:NewsScraper
    {
        public YahooFinance()
            : base("Yahoo Finance")
        {

        }

        protected override void GetLinks(string symbol)
        {
            string url = "https://au.finance.yahoo.com/q/h?s=" + symbol; //&t=2014-11-04T13:04:09-05:00

            string html = Utilities.MakeRequest(url);

            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(html);

            HtmlNode node = doc.DocumentNode;

        }
    }
}
