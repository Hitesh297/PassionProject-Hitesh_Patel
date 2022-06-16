namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFieldsForImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "EmployeeHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Employees", "PicExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "PicExtension");
            DropColumn("dbo.Employees", "EmployeeHasPic");
        }
    }
}
