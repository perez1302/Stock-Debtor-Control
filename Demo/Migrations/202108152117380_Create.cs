namespace Demo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.User1", "DebtorMaster_AccCode", "dbo.DebtorMasters");
            DropIndex("dbo.User1", new[] { "DebtorMaster_AccCode" });
            DropTable("dbo.User1");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.User1",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        IDNo = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        AccType = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        ConfirmPassword = c.String(nullable: false),
                        DebtorMaster_AccCode = c.Int(),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateIndex("dbo.User1", "DebtorMaster_AccCode");
            AddForeignKey("dbo.User1", "DebtorMaster_AccCode", "dbo.DebtorMasters", "AccCode");
        }
    }
}
