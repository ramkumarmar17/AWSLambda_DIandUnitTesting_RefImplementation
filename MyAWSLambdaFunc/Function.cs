using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MyAWSLambdaFunc
{
    public class MyFunction
    {
        private IConfiguration _configuration { get; set; }
        private IMyIPService _ipService { get; set; }

        // This ctor is used when Lambda execution environment is initialized
        public MyFunction()
        {
            // Set up Dependency Injection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Get Dependencies from DI system
            _configuration = serviceProvider.GetService<IConfiguration>();
            _ipService = serviceProvider.GetService<IMyIPService>();
        }

        // This ctor is used in unit tests that can mock IConfigurationService & IMyIPService
        public MyFunction(IConfiguration config, IMyIPService service)
        {
            _configuration = config;
            _ipService = service;
        }

        public string FunctionHandler(string input, ILambdaContext context)
        {
            return _ipService.GetIPInfo(input);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register services with DI system

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration config = configurationBuilder.Build();

            var ipServiceUrlTemplate = config["IPService:urlTemplate"];

            services.AddTransient<IConfiguration>(sp => { return config; });
            services.AddTransient<IMyIPService>(sp => new MyIPService(ipServiceUrlTemplate));
        }
    }
}
