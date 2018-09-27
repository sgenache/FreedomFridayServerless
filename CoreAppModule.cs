using System.IO;
using System.Net.Http;
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
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("config.json")
            //    .Build();

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddSingleton<ISchema>(s => new Schema { Query = new StarWarsQuery() });

            //services.AddSingleton<HttpClient>();
        }
    }

    public class Module : IModule
    {
        public virtual void Load(IServiceCollection services)
        {
            return;
        }
    }

    public interface IModule
    {
        void Load(IServiceCollection services);
    }
}