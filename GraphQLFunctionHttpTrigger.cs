using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GraphQL.Types;
using GraphQL;
using GraphQL.Http;
using System.Linq;
using FreedomFridayServerless.DependencyInjection;

namespace FreedomFridayServerless.Function
{
    public static class GraphQLFunctionHttpTrigger
    {
        public static IFunctionFactory Factory = new CoreFunctionFactory(new CoreAppModule());

        [FunctionName("GraphQLFunctionHttpTrigger")]
        public static Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = "graphql")]HttpRequest req, ILogger log)
        {
            log.LogInformation("GraphQLFunctionHttpTrigger processed a request.");

            return Factory.Create<IGraphQLFunction>(log)
                .InvokeAsync<HttpRequest, IActionResult>(req);
        }
    }

}
