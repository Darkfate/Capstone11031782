namespace Capstone.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BingNewsMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BingNews",
                    c => new
                    {
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        Id = c.Guid(false, true),
                        Title = c.String(true, 125),
                        Url = c.String(true, 125),
                        Source = c.String(true, 125),
                        Description = c.String(true, 125),
                        Date = c.DateTime(true)
                    })
                    .PrimaryKey(t => t.Id).ForeignKey("dbo.Companies", t => t.CompanyId);
                    }
        
        public override void Down()
        {
        }
    }
}
