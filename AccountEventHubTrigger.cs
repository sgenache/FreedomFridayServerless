using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FreedomFridayServerless.Function
{
    public static class AccountEventHubTrigger
    {
        [FunctionName("AccountEventHubTrigger")]
        public static void Run([EventHubTrigger("FreedomFriday", 
            Connection = "EventHubConnectionAppSetting")]EventData eventMessage, 
            ILogger log)
        {
            log.LogInformation($"C# Event Hub trigger function processed a message: {eventMessage.ToString()}");
        }
    }
}
