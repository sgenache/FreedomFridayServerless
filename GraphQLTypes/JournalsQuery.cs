using System;
using GraphQL.Types;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class JournalsQuery : ObjectGraphType
    {
        public JournalsQuery()
        {
            Field<JournalType>()
                .Name("journal")
                .Description("Get journal by Id")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The Id of the journal.")
                .ResolveAsync(ctx =>
                {
                    throw new NotImplementedException();
                });

            Connection<JournalType>()
                .Name("all")
                .Description("Get all journals")
                .Argument<DateGraphType>("fromDate", "The Date from when to get Journals.")
                .Argument<DateGraphType>("toDate", "The Date until to get Journals.")
                .Argument<DateTimeGraphType>("updatedSince", "The Updated DateTime at when to get Journals.")
                .Argument<StringGraphType>("search", "Provides the match criteria for the Transaction search fields: Reference, Number, SourceType, AccountCode, AccountName, LineDescription, LineDebit, LineCredit.")
                .Unidirectional()
                .PageSize(5)
                .ResolveAsync(ctx =>
                {
                    throw new NotImplementedException();
                });
        }
    }
}