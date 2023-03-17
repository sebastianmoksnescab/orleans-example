using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers.Streams.AzureQueue;

internal class Program
{
	private static async Task<int> Main(string[] args)
	{
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

					Action<SiloAzureQueueStreamConfigurator> queueConfig = configurator =>
					{
						configurator.ConfigureAzureQueue(
							ob => ob.Configure(options =>
							{
								options.ConfigureQueueServiceClient(connectionString);
								options.QueueNames = new List<string> { "yourprefix-azurequeueprovider-0" };
							}));
						configurator.ConfigureCacheSize(1024);
						//configurator.ConfigurePullingAgent(ob => ob.Configure(options =>
						//{
						//	options.GetQueueMsgsTimerPeriod = TimeSpan.FromMilliseconds(200);
						//}));
					};



					builder.Configure<ClusterOptions>(options =>
					{
						options.ClusterId = Constants.ClusterId;
						options.ServiceId = Constants.ServiceId;
					});
					builder.UseAzureStorageClustering(options => options.ConfigureTableServiceClient(connectionString));
					builder.AddAzureTableGrainStorage("vinSearchStore", options => options.ConfigureTableServiceClient(connectionString));
					builder.AddAzureQueueStreams("AzureQueueProvider", queueConfig);
					builder.AddLogStorageBasedLogConsistencyProvider();
				});

			var host = builder.Build();
			await host.StartAsync();

			return host;
		}
	}
}