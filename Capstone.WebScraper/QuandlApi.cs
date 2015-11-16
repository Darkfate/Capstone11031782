using Capstone.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.WebScraper
{
    public class QuandlApi
    {
        private const string APIKey = "mi-rsf_-SJYgm_xqm27i";
        private const string Url = "https://www.quandl.com/api/v1/";
        // Example call https://www.quandl.com/api/v1/datasets/WIKI/AAPL.csv?sort_order=asc&exclude_headers=true&rows=3&trim_start=2012-11-01&trim_end=2013-11-30&column=4&collapse=quarterly&transformation=rdiff
        // https://www.quandl.com/api/v1/datasets.json?query=crude+oil
        // https://www.quandl.com/api/v1/datasets/WIKI/AAPL.json

        public void ScrapeAllPrice(string companySymbol)
        {
            string apiUrl = string.Format("{0}datasets/WIKI/{1}.json?auth_token={2}", Url, companySymbol, APIKey);

            var jsonResponseString = GET(apiUrl);

            var jsonResponse = JsonConvert.DeserializeObject<QuandlJsonResponse>(jsonResponseString);

            var data = jsonResponse.Data;

            foreach (var entry in data)
            {
                if (entry.Length == 13)
                {
                    QuandlStockPrice price = new QuandlStockPrice()
                    {
                        CompanyId = companySymbol,
                        Date = DateTime.Parse(entry[0].ToString()),
                        Open = (double?)entry[1],
                        High = (double?)entry[2],
                        Low = (double?)entry[3],
                        Close = (double?)entry[4],
                        Volume = (double?)entry[5],
                        ExDividend = (double?)entry[6],
                        SplitRatio = (double?)entry[7],
                        AdjOpen = (double?)entry[8],
                        AdjHigh = (double?)entry[9],
                        AdjLow = (double?)entry[10],
                        AdjClose = (double?)entry[11],
                        AdjVolume = (double?)entry[12]
                    };

                    using (var db = new CapstoneContext())
                    {
                        try
                        {
                            db.QStockPrices.Add(price);
                            db.SaveChanges();
                            Console.WriteLine(companySymbol + " " + price.Date.ToShortDateString());
                        }
                        catch
                        {
                            Console.WriteLine("FAILED!! " + companySymbol + " " + price.Date.ToShortDateString());
                        }
                    }


                }
                else
                {
                    throw new Exception();
                }
            }
        }

        // Returns JSON string
        public string GET(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        private class QuandlJsonResponse
        {
            public List<object[]> Data { get; set; }
        }

        public void SaveCompanyCSV(string companySymbol)
        {
            string apiUrl = string.Format("{0}datasets/WIKI/{1}.csv?auth_token={2}", Url, companySymbol, APIKey);

            string saveLocation = System.Configuration.ConfigurationManager.AppSettings["DownloadPath"] + "Results\\";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);

                var response = request.GetResponse();

                string directory = saveLocation + companySymbol + "\\";

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var responseStream = response.GetResponseStream();
                string filePath = directory + companySymbol + ".csv";
                var fileStream = File.Create(filePath);
                responseStream.CopyTo(fileStream);

                fileStream.Close();

                Console.WriteLine(companySymbol + " Success");
            }
            catch (Exception e)
            {
            }

        }
    }

    public class QuandlSetUp
    {
        public List<Company> GetCompaniesFromQuandlCSV()
        {
            var companies = new List<Company>();
            using (var fileReader = new StreamReader("WIKI_tickers.csv"))
            {
                while (!fileReader.EndOfStream)
                {
                    string line = fileReader.ReadLine();

                    if (line.Contains("WIKI"))
                    {
                        string cleanedStr = line.Replace("WIKI/", string.Empty).Replace(",\"", "$").Replace("\"", string.Empty);
                        var splitString = cleanedStr.Split('$');

                        companies.Add(new Company() {StockId = splitString[0], CompanyName = splitString[1] });
                    }
                }
            }

            return companies;
        }

        public void SaveCompanyList(List<Company> companies)
        {
            foreach (var c in companies)
            {

                using (var db = new CapstoneContext())
                {
                    db.Companies.Add(c);
                    db.SaveChanges();
                }

                Console.WriteLine(c.StockId);
            }
        }
    }
}
