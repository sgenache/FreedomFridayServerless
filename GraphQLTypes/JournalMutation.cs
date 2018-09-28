using System.Net.Http;
using System.Text;
using System.Web.Http;
using FreedomFridayServerless.Configuration;
using FreedomFridayServerless.Contracts;
using GraphQL.Types;
using Newtonsoft.Json;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class JournalMutation : ObjectGraphType
    {
        public JournalMutation(
            JournalSettings journalSettings,
            HttpClient httpClient)
        {
            Field<JournalType>()
                .Name("createJournal")
                .Argument<NonNullGraphType<JournalInputType>>("journal", "Add a journal")
                .ResolveAsync(async ctx => 
                {
                    var dto = ctx.GetArgument<JournalDTO>("journal");

                    var requestUrl = $"{journalSettings.BaseUrl}{journalSettings.Endpoints.AddJournal}";
                    var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.Default, "application/json");

                    using (var responseMessage = await httpClient.PostAsync(requestUrl, content))
                    {                
                        if (!responseMessage.IsSuccessStatusCode) 
                        {
                            var ex = responseMessage.Content.ReadAsAsync<HttpError>().Exception;
                            ctx.Errors.Add(new GraphQL.ExecutionError("error", ex));
                            return null;
                        }

                        var serializedResponse = await responseMessage.Content.ReadAsStringAsync();
                        var response = JsonConvert.DeserializeObject<object>(serializedResponse);
                        return response;
                    }
                });
        }
    }
}