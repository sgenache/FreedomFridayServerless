using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FreedomFridayServerless.Contracts;
using FreedomFridayServerless.Domain.Core;
using FreedomFridayServerless.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FreedomFridayServerless.Function
{
    public static class DurableFunctionsOrchestration
    {
        [FunctionName("DurableFunctionsOrchestration")]
        public static async Task<Result<JournalDTO>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var dto = context.GetInput<JournalDTO>();

            var journalResult = await context.CallActivityAsync<Result<JournalDTO>>("DurableFunctionsOrchestration_Journal", dto);
            if (journalResult.IsFailure) return journalResult;

            var events = journalResult.Value
                .Lines
                .Select(line => line.ToPostedEvent(journalResult.Value.Id, journalResult.Value.Date));
           
            var tasks = new List<Task>();

            foreach(var @event in events)
            {
                tasks.Add(context.CallActivityAsync("DurableFunctionsOrchestration_Account", @event));
                tasks.Add(context.CallActivityAsync("DurableFunctionsOrchestration_TaxCategory", @event));
            }

            try 
            {
                await Task.WhenAll(tasks);
            }
            catch(Exception)
            {
                //compensating transactions 
                throw;
            }

            return journalResult;
        }

        [FunctionName("DurableFunctionsOrchestration_Journal")]
        public static async Task<Result<JournalDTO>> AddJournal(
            [ActivityTrigger] JournalDTO dto,
            [CosmosDB(
                databaseName: "FreedomFriday",
                collectionName: "journals",
                ConnectionStringSetting = "freedomfridayserverless_DOCUMENTDB")]IAsyncCollector<JournalDTO> journalStore, 
            ILogger log)
        {
            log.LogInformation("DurableFunctionsOrchestration_Journal function processed a request.");

            var result = JournalBuilder
                .Init()
                .WithTransactionId(Guid.NewGuid())
                .WithDate(dto.Date)
                .WithTransactionLines(dto.Lines.Select(l => JournalLineBuilder
                    .Init()
                    .WithDebitCreditAmount(l.AmountDebit, l.AmountCredit)
                    .WithDate(dto.Date)
                    .WithAccountSourceId(l.AccountId)
                    .WithAccountCode(l.AccountCode)
                    .WithAccountName(l.AccountName)
                    .WithDescription(l.Description)))
                .WithNumber(dto.Number)
                .WithReference(dto.Reference)
                .WithDateUpdated(DateTime.UtcNow)
                .Build()
                .OnSuccess(journal => Result.Ok(journal.ToJournalDto()));
            if (result.IsFailure) return result;
            
            await journalStore.AddAsync(result.Value);
            return result;
        }

        [FunctionName("DurableFunctionsOrchestration_Account")]
        public static void UpdateAccount([ActivityTrigger] JournalPostedEvent @event, 
            ILogger log)
        {   
            var body = JsonConvert.SerializeObject(@event);
            log.LogInformation($"DurableFunctionsOrchestration_Account function processed a message: {body}");

            // Update Account Balance
        }

        [FunctionName("DurableFunctionsOrchestration_TaxCategory")]
        public static void UpdateTaxCategory([ActivityTrigger] JournalPostedEvent @event,
            ILogger log)
        {
            var body = JsonConvert.SerializeObject(@event);
            log.LogInformation($"DurableFunctionsOrchestration_TaxCategory function processed a message: {body}");
            
             // Update Tax Category Balance
        }

        [FunctionName("DurableFunctionsOrchestration_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            var stream = await req.Content.ReadAsStreamAsync();
            var dto = stream.Deserialize<JournalDTO>();

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("DurableFunctionsOrchestration", dto);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            var response = starter.CreateCheckStatusResponse(req, instanceId);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(2));
            return response;
        }
    }
}