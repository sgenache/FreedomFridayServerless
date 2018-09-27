using Microsoft.Extensions.DependencyInjection;

namespace FreedomFridayServerless.DependencyInjection
{
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