using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ParallelProcessing
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register BespokeDictionary for the IBespokeDictionary interface
            services.AddSingleton<IBespokeDictionary, BespokeDictionary>();
        }
    }
}
