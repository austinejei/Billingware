namespace Billingware.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.String(nullable: false, maxLength: 50),
                        Alias = c.String(),
                        Extra = c.String(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.AccountNumber);
            
            CreateTable(
                "dbo.Conditions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ConditionApplicatorType = c.Int(nullable: false),
                        AppliedToAccountNumbers = c.String(),
                        Key = c.Int(nullable: false),
                        KeyExpression = c.String(),
                        Type = c.Int(nullable: false),
                        Value = c.String(),
                        OutcomeId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        ConditionConnector = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConditionOutcomes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OutcomeType = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.String(maxLength: 50),
                        CreatedAt = c.DateTime(nullable: false),
                        Narration = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceBefore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceAfter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionType = c.Int(nullable: false),
                        SatisfiedCondition = c.Boolean(nullable: false),
                        ConditionFailReason = c.String(),
                        Honoured = c.Boolean(nullable: false),
                        ClientId = c.String(),
                        Reference = c.String(maxLength: 50),
                        Ticket = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.AccountNumber)
                .Index(t => t.Reference)
                .Index(t => t.Ticket);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Transactions", new[] { "Ticket" });
            DropIndex("dbo.Transactions", new[] { "Reference" });
            DropIndex("dbo.Transactions", new[] { "AccountNumber" });
            DropIndex("dbo.Accounts", new[] { "AccountNumber" });
            DropTable("dbo.Transactions");
            DropTable("dbo.ConditionOutcomes");
            DropTable("dbo.Conditions");
            DropTable("dbo.Accounts");
        }
    }
}
