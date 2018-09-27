using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FreedomFridayServerless.Function
{
    public interface IFunctionFactory
    {
        TFunction Create<TFunction>(ILogger log) where TFunction : IFunction;
    }

    public interface IFunction
    {
        Task<TOutput> InvokeAsync<TInput, TOutput>(TInput input) where TInput: class
                                                                 where TOutput: class;
    }
}