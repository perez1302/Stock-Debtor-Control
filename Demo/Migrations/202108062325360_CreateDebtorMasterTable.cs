namespace Demo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDebtorMasterTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DebtorMasters",
                c => new
                    {
                        AccCode = c.Int(nullable: false, identity: true),
                        DebtorName = c.String(nullable: false),
                        DebtorSurname = c.String(nullable: false),
                        IDNo = c.String(nullable: false, maxLength: 13),
                        DebtorsAddress = c.String(nullable: false),
                        Suburb = c.String(nullable: false),
                        Town = c.String(nullable: false),
                        Province = c.String(nullable: false),
                        CellNo = c.String(nullable: false, maxLength: 10),
                        email = c.String(nullable: false),
                        Balance = c.Double(nullable: false),
                        SalesToDate = c.Double(nullable: false),
                        CostToDate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.AccCode);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DebtorMasters");
        }
    }
}
