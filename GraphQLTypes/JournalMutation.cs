using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FreedomFridayServerless.Configuration;
using FreedomFridayServerless.Contracts;
using FreedomFridayServerless.Domain.Core;
using GraphQL.Types;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace FreedomFridayServerless.GraphQLTypes
{
    public class JournalMutation : ObjectGraphType
    {
        private readonly HttpClient httpClient;

        public JournalMutation(
            JournalSettings journalSettings,
            OrchestratorSettings orchestratorSettings,
            HttpClient httpClient)
        {
            Field<JournalType>()
                .Name("createJournalEC")
                .Argument<NonNullGraphType<JournalInputType>>("journal", "Add a journal")
                .ResolveAsync(async ctx => 
                {
                    var dto = ctx.GetArgument<JournalDTO>("journal");

                    var requestUrl = $"{journalSettings.BaseUrl}/{journalSettings.Endpoints.AddJournal}";
                    var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.Default, "application/json");

                    using (var responseMessage = await httpClient.PostAsync(requestUrl, content))
                    {                
                        if (!responseMessage.IsSuccessStatusCode) 
                        {           
                            var errorContent = await responseMessage.Content.ReadAsStringAsync();
                            if (!string.IsNullOrEmpty(errorContent)) ctx.Errors.Add(new GraphQL.ExecutionError(errorContent));
                            else responseMessage.EnsureSuccessStatusCode();
                            return null;
                        }

                        var serializedResponse = await responseMessage.Content.ReadAsStringAsync();
                        var response = JsonConvert.DeserializeObject<JournalDTO>(serializedResponse);
                        return response;
                    }
                });

            Field<JournalType>()
                .Name("createJournalSaga")
                .Argument<NonNullGraphType<JournalInputType>>("journal", "Add a journal")
                .ResolveAsync(async ctx => 
                {
                    var dto = ctx.GetArgument<JournalDTO>("journal");

                    var requestUrl = $"{orchestratorSettings.BaseUrl}/{orchestratorSettings.FunctionName}";
                    var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.Default, "application/json");

                    HttpManagementPayload httpManagementPayload;
                    using (var responseMessage = await httpClient.PostAsync(requestUrl, content))
                    {                
                        if (!responseMessage.IsSuccessStatusCode) 
                        {           
                            var errorContent = await responseMessage.Content.ReadAsStringAsync();
                            if (!string.IsNullOrEmpty(errorContent)) ctx.Errors.Add(new GraphQL.ExecutionError(errorContent));
                            else responseMessage.EnsureSuccessStatusCode();
                            return null;
                        }

                        var serializedResponse = await responseMessage.Content.ReadAsStringAsync();
                        httpManagementPayload = JsonConvert.DeserializeObject<HttpManagementPayload>(serializedResponse);
                    }
                    
                    JToken status = null;

                    do
                    {
                        await Task.Delay(2000);
                        status = await GetStatus(httpManagementPayload.StatusQueryGetUri, ctx);
                        if (status == null) return status;
                        
                    } while(status.Value<string>("runtimeStatus") == "Pending" || 
                            status.Value<string>("runtimeStatus") == "Running");

                    if (status.Value<string>("runtimeStatus") != "Completed")
                    {
                        ctx.Errors.Add(new GraphQL.ExecutionError("Saga failed"));
                        return null;
                    }
                    var journalResult = status["output"].ToObject<Result<JournalDTO>>();

                    if (journalResult.IsFailure)
                    {
                        ctx.Errors.Add(new GraphQL.ExecutionError(journalResult.ErrorMessage));
                        return null;
                    }
                    return journalResult.Value;

                });
            this.httpClient = httpClient;
        }    

        private async Task<JToken> GetStatus(string statusQueryGetUri, ResolveFieldContext<object> ctx)
        {
            using (var responseMessage = await this.httpClient.GetAsync(statusQueryGetUri))
            {
                if (!responseMessage.IsSuccessStatusCode) 
                {           
                    var errorContent = await responseMessage.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(errorContent)) ctx.Errors.Add(new GraphQL.ExecutionError(errorContent));
                    else responseMessage.EnsureSuccessStatusCode();
                    return null;
                }

                var status = await responseMessage.Content.ReadAsStringAsync();
                return JToken.Parse(status);
            }
        }  
    }
}