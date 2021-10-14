namespace Demo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateStockTransactionFileTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockTransactionFiles",
                c => new
                    {
                        StockTransactionID = c.Int(nullable: false, identity: true),
                        StockCode = c.Int(nullable: false),
                        StockDes = c.String(),
                        Quantity = c.Int(nullable: false),
                        UnitCost = c.Double(nullable: false),
                        UnitSP = c.Double(nullable: false),
                        Total = c.Double(nullable: false),
                        StockTransactionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.StockTransactionID)
                .ForeignKey("dbo.StockMasters", t => t.StockCode, cascadeDelete: true)
                .Index(t => t.StockCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockTransactionFiles", "StockCode", "dbo.StockMasters");
            DropIndex("dbo.StockTransactionFiles", new[] { "StockCode" });
            DropTable("dbo.StockTransactionFiles");
        }
    }
}
