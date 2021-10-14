namespace Demo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateInvoiceDebtTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InvoiceDebts",
                c => new
                    {
                        InvoiceNo = c.Int(nullable: false, identity: true),
                        AccCode = c.Int(nullable: false),
                        Tid = c.Int(nullable: false),
                        Stock = c.String(),
                        UnitPrice = c.Double(nullable: false),
                        quantity1 = c.Int(nullable: false),
                        TotalExl = c.Double(nullable: false),
                        VatAmo = c.Double(nullable: false),
                        Total = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.InvoiceNo)
                .ForeignKey("dbo.DebtorMasters", t => t.AccCode, cascadeDelete: true)
                .Index(t => t.AccCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceDebts", "AccCode", "dbo.DebtorMasters");
            DropIndex("dbo.InvoiceDebts", new[] { "AccCode" });
            DropTable("dbo.InvoiceDebts");
        }
    }
}
