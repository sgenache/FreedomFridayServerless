using System;
using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class AccountsQuery : ObjectGraphType
    {
        public AccountsQuery()
        {
            Field<AccountType>()
                .Name("account")
                .Description("Get Account by Id")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The Id of the Account.")
                .ResolveAsync(ctx =>
                {               
                    throw new NotImplementedException();
                });

            Connection<AccountType>()
                .Name("all")
                .Description("Get all accounts")
                .Argument<DateTimeGraphType>("updatedSince", "The Updated DateTime at when to get Accounts.")
                .Argument<StringGraphType>("search", "Provides the match criteria for the Account search fields: Code, Name, SourceClassification.")
                .Unidirectional()
                .PageSize(5)
                .ResolveAsync(ctx =>
                {
                    throw new NotImplementedException();
                });
        }
    }
}