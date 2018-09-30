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
                .AddJsonFile("local.settings.json", optional:true, reloadOnChange:true)
                .Build();

            var hostName = config.GetSection("WEBSITE_HOSTNAME").Value;
            var baseUrl = hostName.Contains("0.0.0.0") 
                ? $"http://{hostName.Replace("0.0.0.0", "localhost")}/api"
                : $"htts://{hostName}/api";

            services.AddSingleton(new OrchestratorSettings 
            {
                BaseUrl = baseUrl,
                FunctionName = config.GetSection("OrchestratorSettings.FunctionName").Value
            });
            services.AddSingleton(new JournalSettings
            {
                BaseUrl = baseUrl,
                Endpoints = new Endpoints { AddJournal = config.GetSection("JournalSettings.AddEndpoint").Value }
            });
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