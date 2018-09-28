using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class AccountingSchema : Schema
    {
        public AccountingSchema(GraphQL.IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<RootQuery>();
            Mutation = resolver.Resolve<JournalMutation>();
        }
    }
}