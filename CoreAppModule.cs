using System.IO;
using System.Net.Http;
using  FreedomFridayServerless.Configuration;
using FreedomFridayServerless.DependencyInjection;
using FreedomFridayServerless.GraphQLTypes;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
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
            var journalSettings = config.Get<Config>().JournalSettings;
            journalSettings.BaseUrl = hostName.Contains("0.0.0.0") 
                ? hostName.Replace("0.0.0.0", "localhost")
                : hostName;

            services.AddSingleton(journalSettings);
            services.AddSingleton<HttpClient>();

            services.AddSingleton<GraphQL.IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            services.AddSingleton<ISchema, AccountingSchema>();   
            services.AddSingleton<AccountType>();
            services.AddSingleton<JournalType>();          
            services.AddSingleton<JournalLineType>();
            services.AddSingleton<JournalInputType>();
            services.AddSingleton<JournalLineInputType>();
            services.AddSingleton<RootQuery>();           
            services.AddSingleton<AccountsQuery>();
            services.AddSingleton<JournalsQuery>();
            services.AddSingleton<JournalMutation>();

            services.AddTransient<IGraphQLFunction, GraphQLFunction>();
        }
    }
}