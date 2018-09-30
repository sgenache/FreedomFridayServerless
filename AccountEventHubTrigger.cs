using System.Text;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FreedomFridayServerless.Function
{
    public static class AccountEventHubTrigger
    {
        [FunctionName("AccountEventHubTrigger")]
        public static void Run([EventHubTrigger("freedomfridaylocal", 
            Connection = "EventHubConnectionAppSetting",
            ConsumerGroup = "account")]EventData eventMessage, 
            ILogger log)
        {
            var body =  Encoding.UTF8.GetString(eventMessage.Body.Array);
            log.LogInformation($"AccountEventHubTrigger function processed a message: {body}");
            
            // Update Account Balance
        }
    }
}
