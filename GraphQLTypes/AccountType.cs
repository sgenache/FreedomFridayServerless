using FreedomFridayServerless.Contracts;
using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class AccountType: ObjectGraphType<AccountDTO>
    {
        public AccountType()
        {
            Name = "Account";
            Field(a => a.Id, type: typeof(NonNullGraphType<IdGraphType>)).Description("The Id of the Account.");
            Field(a => a.Code).Description("The Account Code.");
            Field(a => a.Name).Description("The Account Name.");
            Field(a => a.IsPnl).Description("True if the Accounts is of type Profit and Loss");
            Field(a => a.UpdatedAt, nullable: true, type: typeof(DateTimeGraphType)).Description("The UpdateAt timestamp.");
        }
    }
}