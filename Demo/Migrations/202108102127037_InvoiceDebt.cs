namespace Demo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceDebt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceDebts", "InvoiceDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InvoiceDebts", "InvoiceDate");
        }
    }
}
