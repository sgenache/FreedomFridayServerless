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
using Newtonsoft.Json.Linq;
using GraphQL.Types;
using GraphQL;
using GraphQL.Http;
using System.Linq;

namespace FreedomFridayServerless.Function
{
    public static class HttpTriggerCSharp
    {
        private static readonly IDocumentExecuter _executer = new DocumentExecuter();
        private static readonly IDocumentWriter _writer = new DocumentWriter();
        private static readonly ISchema _schema = new Schema { Query = new StarWarsQuery() }; 

        [FunctionName("HttpTriggerCSharp")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,"post", Route = "graphql")]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            var request = Deserialize<GraphQLRequest>(req.Body);

            var result = await _executer.ExecuteAsync(_ =>
            {
                _.Schema = _schema;
                _.Query = request.Query;
                _.OperationName = request.OperationName;
                _.Inputs = request.Variables.ToInputs();
                //_.UserContext = _settings.BuildUserContext?.Invoke(context);
            });

            var json = _writer.Write(result);
            return result.Errors?.Any() == true 
                ? new BadRequestObjectResult(json) 
                : (ActionResult)new OkObjectResult(json);
        }

        private static T Deserialize<T>(Stream s)
        {
            using (var reader = new StreamReader(s))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var ser = new JsonSerializer();
                return ser.Deserialize<T>(jsonReader);
            }
        }  
    }

    public class GraphQLRequest
    {
        public string OperationName { get; set; }
        public string Query { get; set; }
        public JObject Variables { get; set; }
    }
}
