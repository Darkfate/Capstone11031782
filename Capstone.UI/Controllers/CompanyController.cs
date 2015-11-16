using Capstone.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone.UI.Controllers
{
    public class CompanyController : Controller
    {
        //
        // GET: /Company/

        public ActionResult Details()
        {
            var symbol = Request["symb"].ToString();
            var context = new CapstoneContext();
            var model = context.Companies.Find(symbol);

            return View(model);
        }

    }
}
