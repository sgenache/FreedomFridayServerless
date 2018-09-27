using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FreedomFridayServerless.DependencyInjection;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FreedomFridayServerless.Function
{
    public interface IGraphQLFunction : IFunction
    {
    }

    public class GraphQLFunction : IGraphQLFunction
    {
        private readonly IDocumentExecuter documentExecuter;
        private readonly IDocumentWriter documentWriter;
        private readonly ISchema schema;

        public GraphQLFunction(
            IDocumentExecuter documentExecuter,
            IDocumentWriter documentWriter,
            ISchema schema)
        {
            this.documentExecuter = documentExecuter;
            this.documentWriter = documentWriter;
            this.schema = schema;
        }

        public ILogger Log { get; set; }

        public async Task<TOutput> InvokeAsync<TInput, TOutput>(TInput input) 
            where TInput: class
            where TOutput: class
        {
            var req = input as HttpRequest;
            string name = req.Query["name"];

            var request = Deserialize<GraphQLRequest>(req.Body);

            var result = await documentExecuter.ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = request.Query;
                _.OperationName = request.OperationName;
                _.Inputs = request.Variables.ToInputs();
                //_.UserContext = _settings.BuildUserContext?.Invoke(context);
            });

            var json = documentWriter.Write(result);
            return result.Errors?.Any() == true 
                ? new BadRequestObjectResult(json) as TOutput
                : new OkObjectResult(json) as TOutput;
        }

        private T Deserialize<T>(Stream s)
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