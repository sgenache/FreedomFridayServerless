using System.Text;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FreedomFridayServerless.Function
{
    public static class TaxCategoryEventHubTrigger
    {
        [FunctionName("TaxCategoryEventHubTrigger")]
        public static void Run([EventHubTrigger("freedomfriday", 
            Connection = "EventHubConnectionAppSetting",
            ConsumerGroup = "taxcategory")]EventData eventMessage, 
            ILogger log)
        {
            var body =  Encoding.UTF8.GetString(eventMessage.Body.Array);
            log.LogInformation($"TaxCategoryEventHubTrigger function processed a message: {body}");
            
             // Update Tax Category Balance
        }
    }
}
