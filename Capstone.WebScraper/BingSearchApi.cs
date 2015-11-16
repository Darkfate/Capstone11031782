using Bing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.WebScraper
{
    public class BingSearchApi
    {
        public BingSearchApi()
        {
            Container = new BingSearchContainer(new Uri(rootUrl));
            Container.Credentials = new NetworkCredential(accountKey, accountKey);
        }

        public List<NewsResult> Query(string company)
        {
            string market = "en-us";
            string options = null;
            string adult = null;
            double? lat = null;
            double? lon = null;
            string newLocation = null;
            string newsCategory = "rt_Business";
            string sortBy = "Date";

            Container = new BingSearchContainer(new Uri(rootUrl));
            Container.Credentials = new NetworkCredential(accountKey, accountKey);

            var query = Container.News(company, options, market, adult, lat, lon, newLocation, newsCategory, sortBy);

            List<NewsResult> totalSearchList = new List<NewsResult>();

            for (int i = 0; i < 4; i++)
            {
                int skip = totalSearchList.Count();
                var offsetQuery = query.AddQueryOption("$skip", skip.ToString());
                var result = offsetQuery.Execute().ToList();
                totalSearchList.AddRange(result);
            }

            return totalSearchList;
        }

        private BingSearchContainer Container { get; set; }
        private const string rootUrl = "https://api.datamarket.azure.com/Bing/Search/v1/";
        private const string accountKey = "4uLGXNhrRRJwsE+e8OozmPZAW9poh6CS//IKAwrjY/s";

    }

    //Primary Account Key 	xNz3dnyxGO2sAlhPtDCa+66wVae3a5C7Jz4wAYull7E
    //Customer ID 	12f16b5a-4490-437b-bf4e-bc6b64ef9361
}
