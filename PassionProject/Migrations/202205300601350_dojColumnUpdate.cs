namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dojColumnUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "DOJ", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "DOJ", c => c.DateTime(nullable: false));
        }
    }
}
