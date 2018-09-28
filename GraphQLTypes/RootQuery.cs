using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery()
        {
            Name = "root";

            Field<AccountsQuery>("accounts", resolve: ctx => new { });
            Field<JournalsQuery>("transactions", resolve: ctx => new { });
            //Field<ReportsQuery>("reports", resolve: ctx => new { });
        }
    }
}