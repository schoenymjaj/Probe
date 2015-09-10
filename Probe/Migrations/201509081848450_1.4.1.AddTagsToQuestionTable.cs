namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _141AddTagsToQuestionTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Question", "Tags", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Question", "Tags");
        }
    }
}
