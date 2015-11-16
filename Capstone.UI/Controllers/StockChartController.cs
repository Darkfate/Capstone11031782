using Capstone.Domain.Models;
using Capstone.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone.UI.Controllers
{
    public class StockChartController : Controller
    {
        //
        // GET: /StockChart/

        [HttpGet]
        public ActionResult Index(string id)
        {
            CapstoneContext context = new CapstoneContext();

            StockViewModel model = new StockViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                model.Company = context.Companies.FirstOrDefault(c => c.StockId == id);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(StockViewModel model)
        {
            if (model.Search != null)
            {
                CapstoneContext context = new CapstoneContext();

                model.Company = context.Companies.FirstOrDefault(c => c.StockId == model.Search);
            }

            return View(model);
        }
    }
}
