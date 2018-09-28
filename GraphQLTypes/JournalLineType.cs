using FreedomFridayServerless.Contracts;
using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class JournalLineType : ObjectGraphType<JournalLineDTO>
    {
        public JournalLineType()
        {
            Name = "JournalLine";
            Field(a => a.AccountId)
                .Description("The Id of the Account.");
            Field(a => a.AccountCode)
                .Description("The Code of the Account.");
            Field(a => a.AccountName)
                .Description("The Description of the Account.");
            Field(a => a.AmountDebit).Description("The Debit amount of the Journal Line.");
            Field(a => a.AmountCredit).Description("The Credit amount of the Journal Line.");
            Field(a => a.Description, nullable:true).Description("The Journal Line description.");
        }
    }

    public class JournalLineInputType : InputObjectGraphType<JournalLineDTO>
    {
        public JournalLineInputType()
        {
            Name = "JournalLineInput";
            Field(a => a.AccountId)
                .Description("The Id of the Account.");
            Field(a => a.AccountCode)
                .Description("The Code of the Account.");
            Field(a => a.AccountName)
                .Description("The Description of the Account.");
            Field(a => a.AmountDebit).Description("The Debit amount of the Journal Line.");
            Field(a => a.AmountCredit).Description("The Credit amount of the Journal Line.");
            Field(a => a.Description, nullable:true).Description("The Journal Line description.");
        }
    }
}