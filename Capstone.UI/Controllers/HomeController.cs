using Capstone.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone.UI.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            var db = new CapstoneContext();

            var model = db.QStockPrices.GroupBy(q => q.CompanyId).Select(g => g.Key);

            return View(model);
        }

        public ActionResult GetSymbol()
        {
            var symbol = Request["symb"].ToString();

            var db = new CapstoneContext();

            var company = db.Companies.FirstOrDefault(e => e.StockId.Equals(symbol, StringComparison.OrdinalIgnoreCase));

            if (company != null)
            {
                @ViewBag.Response = company.CompanyName;
            }
            else
            {
                @ViewBag.Response = "Cannot find company";
            }
            return View("Index");
        }
    }
}
