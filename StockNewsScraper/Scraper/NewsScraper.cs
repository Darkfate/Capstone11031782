using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockNewsScraper.Scraper
{
    public class NewsScraper:INewsScraper
    {
        private string _name { get; set; }

        public NewsScraper(string name)
        {
            _name = name;
            saveLocation = System.Configuration.ConfigurationManager.AppSettings["DownloadPath"] + "Results/" + _name + "/";

            // Date functionalities have not been implemented yet.
            StartDate = new DateTime();
            EndDate = new DateTime();
        }

        public override string ToString()
        {
            return _name;
        }

        // Implementaion to collect data
        public virtual void Scrape(string symbol)
        {
            saveLocation = System.Configuration.ConfigurationManager.AppSettings["DownloadPath"] + "Results/" + symbol + "/" + _name + "/";

            GetLinks(symbol);
        }

        // Implementation to collect links from source
        protected virtual void GetLinks(string symbol)
        {
            
        }

        // Save articles
        protected void Save(string title, string body, DateTime date)
        {
            if (!Directory.Exists(saveLocation))
                Directory.CreateDirectory(saveLocation);

            using (var writer = new StreamWriter(saveLocation + date.ToString("yyyy-MM-dd_") + Utilities.RemoveIlligalChar(title) + ".txt"))
            {
                string shrinkBody = Regex.Replace(body, @"\s+", " ");
                writer.WriteLine(title);
                writer.WriteLine(date.ToString("yyyy-MM-dd hh:mm"));
                writer.WriteLine(shrinkBody);
            }
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        private string saveLocation { get; set; }
    }
}
