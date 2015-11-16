namespace Capstone.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuandlStockPrice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuandlStockPrices",
                c => new
                    {
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Open = c.Double(nullable: false),
                        High = c.Double(nullable: false),
                        Low = c.Double(nullable: false),
                        Close = c.Double(nullable: false),
                        Volume = c.Double(nullable: false),
                        ExDividend = c.Double(nullable: false),
                        SplitRatio = c.Double(nullable: false),
                        AdjOpen = c.Double(nullable: false),
                        AdjHigh = c.Double(nullable: false),
                        AdjLow = c.Double(nullable: false),
                        AdjClose = c.Double(nullable: false),
                        AdjVolume = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.CompanyId, t.Date })
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuandlStockPrices", "CompanyId", "dbo.Companies");
            DropIndex("dbo.QuandlStockPrices", new[] { "CompanyId" });
            DropTable("dbo.QuandlStockPrices");
        }
    }
}
