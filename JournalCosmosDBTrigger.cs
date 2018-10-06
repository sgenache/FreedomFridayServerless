using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreedomFridayServerless.Contracts;
using Microsoft.Azure.Documents;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
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
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, 
            [EventHub("freedomfriday", 
            Connection = "EventHubConnectionAppSetting")] ICollector<EventData> outputMessages, 
            ILogger log)
        {
            log.LogInformation("Documents modified " + input.Count);
            var events = input
                .Select(d => JsonConvert.DeserializeObject<JournalDTO>(d.ToString()))
                .SelectMany(journal => journal
                    .Lines
                    .Select(line => line.ToPostedEvent(journal.Id, journal.Date))
                    .Select(postedEvent => JsonConvert.SerializeObject(postedEvent))
                    .Select(s => new EventData(Encoding.UTF8.GetBytes(s))));
            
            foreach (var @event in events)
            {
                outputMessages.Add(@event);
            }
        }
    }
}
