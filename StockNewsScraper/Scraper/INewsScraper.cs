using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockNewsScraper.Scraper
{
    public interface INewsScraper
    {
        void Scrape(string symbol);
    }
}
