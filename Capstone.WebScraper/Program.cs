using System.Linq;
using Capstone.Domain.Models;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
namespace Capstone.WebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new QuandlApi();

            var mw = new MarketWatchScraper();

            using (var fileReader = new StreamReader(System.Configuration.ConfigurationManager.AppSettings["DownloadPath"] + "List.txt"))
            {
                while (!fileReader.EndOfStream)
                {
                    string line = fileReader.ReadLine();

                    api.SaveCompanyCSV(line.Trim());
                    mw.GetTop100MarketWatchNews(line.Trim());
                    mw.ScrapeMarketWatch(line);

                    Console.WriteLine(line + " Complete");
                    Thread.Sleep(1000);
                }
            }

        }

        private static void QuandlSequentialScrape()
        {
            var api = new QuandlApi();

            List<Company> companies = new List<Company>();

            var scraped = new List<string>();

            using (var db = new CapstoneContext())
            {
                companies = db.Companies.ToList();

                scraped = db.QStockPrices.GroupBy(p => p.CompanyId).Select(p => p.Key).ToList();
            }

            foreach (var company in companies)
            {

                using (var db = new CapstoneContext())
                {
                    scraped = db.QStockPrices.GroupBy(p => p.CompanyId).Select(p => p.Key).ToList();
                }


                if (!scraped.Contains(company.StockId))
                {
                    api.ScrapeAllPrice(company.StockId);
                }
                else
                {
                    Console.WriteLine(company.StockId + " FINISHED");
                }
            }
        }

        private static void scrapOneCompany()
        {
            var api = new QuandlApi();

            List<Company> companies = new List<Company>();

            var scraped = new List<string>();

            using (var db = new CapstoneContext())
            {
                companies = db.Companies.ToList();

                scraped = db.QStockPrices.GroupBy(p => p.CompanyId).Select(p => p.Key).ToList();
            }

            var com = companies.First(c => !(scraped.Contains(c.StockId)));

            api.ScrapeAllPrice(com.StockId);

            Console.WriteLine("Finished");
            Console.ReadLine();

        }

        private static void SaveBingNews()
        {
            var bingSearch = new BingSearchApi();
            //a.Query("AAPL");

            List<Company> companies = new List<Company>();

            using (var db = new CapstoneContext())
            {
                companies = db.Companies.ToList();
            }

            foreach (var company in companies)
            {
                var results = bingSearch.Query(company.CompanyName).
                    Select(
                        news =>
                        {
                            return
                                new BingNews()
                                {
                                    Id = news.ID,
                                    Title = news.Title,
                                    Url = news.Url,
                                    Source = news.Source,
                                    Description = news.Description,
                                    Date = news.Date,
                                    CompanyId = company.StockId
                                };
                        });

                foreach (var entry in results)
                {
                    using (var db = new CapstoneContext())
                    {
                        db.BingNews.Add(entry);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static void CreatedCompanyList()
        {
            var a = new QuandlSetUp();
            var companyList = a.GetCompaniesFromQuandlCSV();
            a.SaveCompanyList(companyList);
        }
    }
}
