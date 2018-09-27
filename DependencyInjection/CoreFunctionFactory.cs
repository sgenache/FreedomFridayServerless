using System;
using System.Threading.Tasks;
using FreedomFridayServerless.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FreedomFridayServerless.DependencyInjection
{
    public class CoreFunctionFactory : IFunctionFactory
    {
        private readonly IServiceProvider _container;

        public CoreFunctionFactory(IModule module = null)
        {
            this._container = new ContainerBuilder()
                .RegisterModule(module)
                .Build();
        }

        public TFunction Create<TFunction>(ILogger log)
            where TFunction : IFunction
        {
            // Resolve the function instance directly from the container.
            var function = this._container.GetService<TFunction>();
            function.Log = log;

            return function;
        }
    }

    public interface IFunctionFactory
    {
        TFunction Create<TFunction>(ILogger log) where TFunction : IFunction;
    }

    public interface IFunction
    {
        ILogger Log {get;set;}

        Task<TOutput> InvokeAsync<TInput, TOutput>(TInput input) where TInput: class
                                                                 where TOutput: class;
    }
}