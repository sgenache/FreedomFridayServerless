using System;
using Microsoft.Extensions.DependencyInjection;

namespace FreedomFridayServerless.DependencyInjection
{
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly IServiceCollection _services;

        public ContainerBuilder()
        {
            this._services = new ServiceCollection();
        }

        public IContainerBuilder RegisterModule(IModule module = null)
        {
            if (module == null)
            {
                module = new Module();
            }

            module.Load(this._services);

            return this;
        }

        public IServiceProvider Build()
        {
            var provider = this._services.BuildServiceProvider();

            return provider;
        }
    }

    public interface IContainerBuilder
    {
        IContainerBuilder RegisterModule(IModule module = null);

        IServiceProvider Build();
    }
}
