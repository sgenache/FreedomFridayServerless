using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery()
        {
            Name = "root";

            Field<AccountsQuery>("accounts", resolve: ctx => new { });
            Field<TransactionsQuery>("transactions", resolve: ctx => new { });
            //Field<ReportsQuery>("reports", resolve: ctx => new { });
        }
    }
}