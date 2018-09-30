using FreedomFridayServerless.Contracts;
using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class TrialBalanceType : ObjectGraphType<TrialBalanceDTO>
    {
        public TrialBalanceType()
        {
            Name = "TrialBalance";
            Field(t => t.Lines, type: typeof(ListGraphType<NonNullGraphType<TrialBalanceLineType>>))
                .Name("Lines")
                .Description("The Trial Balance items");
        }
    }
}