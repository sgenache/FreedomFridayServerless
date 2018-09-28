using FreedomFridayServerless.Contracts;
using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class JournalType : ObjectGraphType<JournalDTO>
    {
        public JournalType()
        {
            Name = "Journal";
            Field(t => t.Id, type: typeof(NonNullGraphType<IdGraphType>)).Description("The Id of the Journal.");       
            Field(t => t.Lines, type: typeof(ListGraphType<NonNullGraphType<JournalLineType>>)).Description("The Journal Lines.");
            Field(t => t.Date).Description("The Date of the Journal.");
            Field(t => t.Reference, nullable:true).Description("The Journal reference.");
            Field(t => t.Number, nullable: true).Description("The Journal number.");
            Field(t => t.UpdatedAt, nullable: true, type: typeof(DateTimeGraphType)).Description("The UpdateAt timestamp.");
        }
    }

    public class JournalInputType : InputObjectGraphType<JournalDTO>
    {
        public JournalInputType()
        {
            Name = "JournalInput";
            //Field(t => t.Id, type: typeof(NonNullGraphType<IdGraphType>)).Description("The Id of the Journal.");       
            Field(t => t.Lines, type: typeof(ListGraphType<NonNullGraphType<JournalLineInputType>>)).Description("The Journal Lines.");
            Field(t => t.Date).Description("The Date of the Journal.");
            Field(t => t.Reference, nullable:true).Description("The Journal reference.");
            Field(t => t.Number, nullable: true).Description("The Journal number.");
        }
    }
}