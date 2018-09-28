using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FreedomFridayServerless.Extensions;
using FreedomFridayServerless.Contracts;
using FreedomFridayServerless.Domain.Core;
using System.Linq;

namespace FreedomFridayServerless.Function
{
    public static class JournalHttpTrigger
    {
        [FunctionName("JournalHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "add")]HttpRequest req,
            [CosmosDB(
                databaseName: "FreedomFriday",
                collectionName: "journals",
                ConnectionStringSetting = "freedomfridayserverless_DOCUMENTDB")]IAsyncCollector<JournalDTO> journalStore, 
            ILogger log)
        {
            log.LogInformation("JournalHttpTrigger function processed a request.");

            var dto = req.Body.Deserialize<JournalDTO>();

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
            if (result.IsFailure) return result.ToActionResult();

            await journalStore.AddAsync(result.Value);
            return result.ToActionResult();
        }
    }
}
