using FreedomFridayServerless.Contracts;
using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class TrialBalanceLineType : ObjectGraphType<TrialBalanceLineDTO>
    {
        public TrialBalanceLineType()
        {
            Name = "Line";
            Field(l => l.AccountId, type: typeof(NonNullGraphType<IdGraphType>))
                .Description("The Id of the Account.");
            Field(l => l.AccountCode)
                .Description("The Code of the Account.");
            Field(l => l.AccountName)
                .Description("The Description of the Account.");
            Field(l => l.Balance)
                .Description("The Net Balance of the Account");
        }
    }
}