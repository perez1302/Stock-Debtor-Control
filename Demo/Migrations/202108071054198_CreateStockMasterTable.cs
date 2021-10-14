namespace Demo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateStockMasterTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockMasters",
                c => new
                    {
                        StockCode = c.Int(nullable: false, identity: true),
                        StockDes = c.String(nullable: false),
                        Cost = c.Double(nullable: false),
                        MarkUp = c.Double(nullable: false),
                        SellingPrice = c.Double(nullable: false),
                        QtyPurchased = c.Double(nullable: false),
                        TotalPurchase = c.Double(nullable: false),
                        QtySold = c.Double(nullable: false),
                        TotalSales = c.Double(nullable: false),
                        StockAvailable = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.StockCode);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.StockMasters");
        }
    }
}
