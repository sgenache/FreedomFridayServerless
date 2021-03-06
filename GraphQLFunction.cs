using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FreedomFridayServerless.DependencyInjection;
using FreedomFridayServerless.Extensions;
using GraphQL;
using GraphQL.Http;
using GraphQL.Instrumentation;
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
            var start = DateTime.UtcNow;

            var req = input as HttpRequest;

            var request = req.Body.Deserialize<GraphQLRequest>();
        
            var result = await documentExecuter.ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = request.Query;
                _.OperationName = request.OperationName;
                _.Inputs = request.Variables.ToInputs();
                _.ExposeExceptions = true;
                _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
                //_.UserContext = _settings.BuildUserContext?.Invoke(context);
            });
            result.EnrichWithApolloTracing(start);

            var json = documentWriter.Write(result);
            return result.Errors?.Any() == true 
                ? new BadRequestObjectResult(json) as TOutput
                : new OkObjectResult(json) as TOutput;
        }
    }

    public class GraphQLRequest
    {
        public string OperationName { get; set; }
        public string Query { get; set; }
        public JObject Variables { get; set; }
    }
}