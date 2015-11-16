using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Domain.Models
{
    public class Company
    {
        [Key]
        public string StockId { get; set; }
        public string CompanyName { get; set; }

        public virtual ICollection<BingNews> BingNews { get; set; }
        public virtual ICollection<QuandlStockPrice> QStockPrice { get; set; }
    }

    public class BingNews
    {
        [ForeignKey("Company")]
        public string CompanyId { get; set; }

        public virtual Company Company { get; set; }

        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Source { get; set; }

        public string Description { get; set; }

        public DateTime? Date { get; set; }
    }

    public class QuandlStockPrice
    {
        [Key]
        [Column(Order = 1)] 
        [ForeignKey("Company")]
        public string CompanyId {get;set;}

        public virtual Company Company { get; set; }

        [Key]
        [Column(Order = 2)] 
        public DateTime Date {get;set;}

        public double? Open { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public double? Close { get; set; }
        public double? Volume { get; set; }
        public double? ExDividend { get; set; }
        public double? SplitRatio { get; set; }
        public double? AdjOpen { get; set; }
        public double? AdjHigh { get; set; }
        public double? AdjLow { get; set; }
        public double? AdjClose { get; set; }
        public double? AdjVolume { get; set; }
    }

    public class CapstoneContext : DbContext
    {
        public CapstoneContext()
            : base("CapstoneDb")
        {
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<BingNews> BingNews { get; set; }
        public DbSet<QuandlStockPrice> QStockPrices { get; set; }
    }
}
