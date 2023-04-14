using System.Security.Cryptography.X509Certificates;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Core;
using Orleans.Providers.Streams.AzureQueue;

try
{
	using IHost host = await StartSiloAsync(args);
	Console.WriteLine("\n\n Press Enter to terminate...\n\n");
	Console.ReadLine();

	await host.StopAsync();

	return 0;
}
catch (Exception ex)
{
	Console.WriteLine(ex);
	return 1;
}

static async Task<IHost> StartSiloAsync(string[] args)
{
	var builder = Host.CreateDefaultBuilder(args)
		.ConfigureAppConfiguration(x => x.AddUserSecrets(typeof(Program).Assembly))
	.UseOrleans(
		(context, builder) =>
		{
			var siloPort = 11111;
			var gatewayPort = 30000;
			var connectionString = context.Configuration["storage"];

			//Action<OptionsBuilder<StreamPullingAgentOptions>> ob = foo =>
			//{
			//	foo.Configure(options =>
			//	{
			//		options.GetQueueMsgsTimerPeriod = TimeSpan.FromMilliseconds(200);
			//	});
			//};

			Action<SiloAzureQueueStreamConfigurator> queueConfig = configurator =>
			{
				configurator.ConfigureAzureQueue(
					ob => ob.Configure(options =>
					{
						//options.ConfigureQueueServiceClient(connectionString)
						options.ConfigureQueueServiceClient(connectionString);
						options.QueueNames = new List<string> { "yourprefix-azurequeueprovider-0" };
					}));
				configurator.ConfigureCacheSize(1024);
				//configurator.ConfigurePullingAgent(ob);
				//configurator.
			};



			builder.Configure<ClusterOptions>(options =>
			{
				options.ClusterId = Constants.ClusterId;
				options.ServiceId = Constants.ServiceId;
			});
				//.AddAzureQueueStreams("AzureQueueProvider", configurator =>
				//{
				//	configurator.ConfigureAzureQueue(
				//		ob => ob.Configure(options =>
				//		{
				//			options. = "xxx";
				//			options.QueueNames = new List<string> { "yourprefix-azurequeueprovider-0" };
				//		}));
				//	configurator.ConfigureCacheSize(1024);
				//	//configurator.ConfigurePullingAgent(ob => ob.Configure(options =>
				//	//{
				//	//	options.GetQueueMsgsTimerPeriod = TimeSpan.FromMilliseconds(200);
				//	//}));
				//});
			//.AddAzureQueueStreams("AzureQueueProvider", configurator =>
			//{
			//	configurator.ConfigureAzureQueue(
			//		ob => ob.Configure(options =>
			//		{
			//			options.ConnectionString = connectionString;
			//			options.QueueNames = new List<string> { "yourprefix-azurequeueprovider-0" };
			//		}));
			//	configurator.ConfigureCacheSize(1024);
			//	configurator.ConfigurePullingAgent(ob => ob.Configure(options =>
			//	{
			//		options.
			//	}));
			//});
			builder.UseAzureStorageClustering(options => options.ConfigureTableServiceClient(connectionString));
			builder.AddAzureTableGrainStorage("vinSearchStore", options => options.ConfigureTableServiceClient(connectionString));
			builder.AddAzureQueueStreams("AzureQueueProvider", queueConfig)
			 .AddAzureTableGrainStorage("PubSubStore", options =>
			 {
				 options.Configure(x =>
				 {
					 x.ConfigureTableServiceClient(connectionString);
				 });
			 });
			builder.AddLogStorageBasedLogConsistencyProvider();
		});

	var host = builder.Build();
	await host.StartAsync();

	return host;
}
