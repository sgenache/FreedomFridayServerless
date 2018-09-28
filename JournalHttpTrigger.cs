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

namespace FreedomFridayServerless.Function
{
    public static class JournalHttpTrigger
    {
        [FunctionName("JournalHttpTrigger")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "add")]HttpRequest req, ILogger log)
        {
            log.LogInformation("JournalHttpTrigger function processed a request.");

            var dto = req.Body.Deserialize<JournalDTO>();

            return new OkObjectResult(dto);
            //return new BadRequestObjectResult("Invalid journal");

            //return name != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
