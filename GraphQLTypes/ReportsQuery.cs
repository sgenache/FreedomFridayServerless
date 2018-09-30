using System;
using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class ReportsQuery : ObjectGraphType
    {
        public ReportsQuery()
        {
            Field<TrialBalanceType>()
                .Name("trialBalance")
                .Description("Get Trial Balance")
                .Argument<NonNullGraphType<DateGraphType>>("date", "The Trial Balance as at Date.")
                .ResolveAsync(ctx =>
                {
                    throw new NotImplementedException();         
                });
        }
    }
}