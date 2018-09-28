using System.Collections.Generic;
using FreedomFridayServerless.Contracts;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FreedomFridayServerless.Function
{
    public static class JournalCosmosDBTrigger
    {
        [FunctionName("JournalCosmosDBTrigger")]
        public static void Run([CosmosDBTrigger(
            databaseName: "FreedomFriday",
            collectionName: "journals",
            ConnectionStringSetting = "freedomfridayserverless_DOCUMENTDB",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
                var journalDto = JsonConvert.DeserializeObject<JournalDTO>(input[0].ToString());
            }
        }
    }
}
