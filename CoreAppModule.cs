using System.IO;
using System.Net.Http;
using FreedomFridayServerless.Configuration;
using FreedomFridayServerless.DependencyInjection;
using FreedomFridayServerless.GraphQLTypes;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using GraphQL.Types.Relay;
using GraphQL.Types.Relay.DataObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FreedomFridayServerless.Function
{
    public class CoreAppModule : Module
    {
        public override void Load(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("config.json")
                .Build();

            var hostName = config.GetSection("WEBSITE_HOSTNAME").Value;
            var baseUrl = hostName.Contains("0.0.0.0") 
                ? $"http://{hostName.Replace("0.0.0.0", "localhost")}/api"
                : $"htts://{hostName}/api";

            var journalSettings = config.Get<Config>().JournalSettings;
            journalSettings.BaseUrl = baseUrl;

            var orchestratorSettings = config.Get<Config>().OrchestratorSettings;
            orchestratorSettings.BaseUrl = baseUrl;

            services.AddSingleton(journalSettings);
            services.AddSingleton(orchestratorSettings);
            services.AddSingleton<HttpClient>();

            services.AddSingleton<GraphQL.IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddTransient(typeof(ConnectionType<>));
            services.AddTransient(typeof(EdgeType<>));
            services.AddTransient(typeof(Edge<>));
            services.AddTransient<PageInfo>();
            services.AddTransient<PageInfoType>();

            services.AddSingleton<ISchema, AccountingSchema>();   
            services.AddSingleton<AccountType>();
            services.AddSingleton<JournalType>();          
            services.AddSingleton<JournalLineType>();
            services.AddSingleton<JournalInputType>();
            services.AddSingleton<JournalLineInputType>();
            services.AddSingleton<TrialBalanceType>();
            services.AddSingleton<TrialBalanceLineType>();
            services.AddSingleton<RootQuery>();           
            services.AddSingleton<AccountsQuery>();
            services.AddSingleton<JournalsQuery>();
            services.AddSingleton<ReportsQuery>();
            services.AddSingleton<JournalMutation>();

            services.AddTransient<IGraphQLFunction, GraphQLFunction>();
        }
    }
}