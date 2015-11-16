namespace Capstone.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PriceUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.QuandlStockPrices", "Open", c => c.Double());
            AlterColumn("dbo.QuandlStockPrices", "High", c => c.Double());
            AlterColumn("dbo.QuandlStockPrices", "Low", c => c.Double());
            AlterColumn("dbo.QuandlStockPrices", "Close", c => c.Double());
            AlterColumn("dbo.QuandlStockPrices", "Volume", c => c.Double());
            AlterColumn("dbo.QuandlStockPrices", "ExDividend", c => c.Double());
            AlterColumn("dbo.QuandlStockPrices", "SplitRatio", c => c.Double());
            AlterColumn("dbo.QuandlStockPrices", "AdjOpen", c => c.Double());
            AlterColumn("dbo.QuandlStockPrices", "AdjHigh", c => c.Double());
            AlterColumn("dbo.QuandlStockPrices", "AdjLow", c => c.Double());
            AlterColumn("dbo.QuandlStockPrices", "AdjClose", c => c.Double());
            AlterColumn("dbo.QuandlStockPrices", "AdjVolume", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.QuandlStockPrices", "AdjVolume", c => c.Double(nullable: false));
            AlterColumn("dbo.QuandlStockPrices", "AdjClose", c => c.Double(nullable: false));
            AlterColumn("dbo.QuandlStockPrices", "AdjLow", c => c.Double(nullable: false));
            AlterColumn("dbo.QuandlStockPrices", "AdjHigh", c => c.Double(nullable: false));
            AlterColumn("dbo.QuandlStockPrices", "AdjOpen", c => c.Double(nullable: false));
            AlterColumn("dbo.QuandlStockPrices", "SplitRatio", c => c.Double(nullable: false));
            AlterColumn("dbo.QuandlStockPrices", "ExDividend", c => c.Double(nullable: false));
            AlterColumn("dbo.QuandlStockPrices", "Volume", c => c.Double(nullable: false));
            AlterColumn("dbo.QuandlStockPrices", "Close", c => c.Double(nullable: false));
            AlterColumn("dbo.QuandlStockPrices", "Low", c => c.Double(nullable: false));
            AlterColumn("dbo.QuandlStockPrices", "High", c => c.Double(nullable: false));
            AlterColumn("dbo.QuandlStockPrices", "Open", c => c.Double(nullable: false));
        }
    }
}
