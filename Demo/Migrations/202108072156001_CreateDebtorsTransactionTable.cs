namespace Demo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDebtorsTransactionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DebtorsTransactions",
                c => new
                    {
                        TransactionID = c.Int(nullable: false, identity: true),
                        AccCode = c.Int(nullable: false),
                        StockCode = c.Int(nullable: false),
                        StockDes = c.String(),
                        SellingPrice = c.Double(nullable: false),
                        Qty = c.Int(nullable: false),
                        GrossValue = c.Double(nullable: false),
                        VatValue = c.Double(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionID)
                .ForeignKey("dbo.DebtorMasters", t => t.AccCode, cascadeDelete: true)
                .ForeignKey("dbo.StockMasters", t => t.StockCode, cascadeDelete: true)
                .Index(t => t.AccCode)
                .Index(t => t.StockCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DebtorsTransactions", "StockCode", "dbo.StockMasters");
            DropForeignKey("dbo.DebtorsTransactions", "AccCode", "dbo.DebtorMasters");
            DropIndex("dbo.DebtorsTransactions", new[] { "StockCode" });
            DropIndex("dbo.DebtorsTransactions", new[] { "AccCode" });
            DropTable("dbo.DebtorsTransactions");
        }
    }
}
