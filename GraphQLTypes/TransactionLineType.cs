using FreedomFridayServerless.Contracts;
using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class TransactionLineType : ObjectGraphType<TransactionLineDTO>
    {
        public TransactionLineType()
        {
            Name = "TransactionLine";
            Field(a => a.AccountCode)
                .Description("The Code of the Account.");
            Field(a => a.AccountName)
                .Description("The Description of the Account.");
            Field(a => a.AmountDebit).Description("The Debit amount of the Transaction Line.");
            Field(a => a.AmountCredit).Description("The Credit amount of the Transaction Line.");
            Field(a => a.Description, nullable:true).Description("The Transaction Line description.");
        }
    }
}