using FreedomFridayServerless.Contracts;
using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class TransactionType : ObjectGraphType<TransactionDTO>
    {
        public TransactionType()
        {
            Name = "Transaction";
            Field(t => t.Id, type: typeof(NonNullGraphType<IdGraphType>)).Description("The Id of the Transaction.");       
            Field(t => t.Lines, type: typeof(ListGraphType<NonNullGraphType<TransactionLineType>>)).Description("The Transaction Lines.");
            Field(t => t.Date).Description("The Date of the Transaction.");
            Field(t => t.Reference, nullable:true).Description("The Transaction reference.");
            Field(t => t.Number, nullable: true).Description("The Transaction number.");
            Field(t => t.UpdatedAt, nullable: true, type: typeof(DateTimeGraphType)).Description("The UpdateAt timestamp.");
        }
    }
}