using Capstone.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.UI.Models
{
    public class StockViewModel
    {
        public Company Company { get; set; }
        public string Search { get; set; }
    }
}