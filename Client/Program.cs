﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GrainInterfaces;
using Microsoft.Extensions.Configuration;
using Orleans.Configuration;
using System.Net;
using Common;
using System.Xml;
using Orleans.Streams;
using Orleans.Providers.Streams.AzureQueue;

try
{
	await Task.Delay(6000);
	using IHost host = await StartClientAsync();
	var client = host.Services.GetRequiredService<IClusterClient>();

	await DoClientWorkAsync(client);
	await host.StopAsync();

	return 0;
}
catch (Exception e)
{
	Console.WriteLine($$"""
        Exception while trying to run client: {{e.Message}}
        Make sure the silo the client is trying to connect to is running.
        Press any key to exit.
        """);

	Console.ReadKey();
	return 1;
}

static async Task<IHost> StartClientAsync()
{
	var builder = new HostBuilder()
		.ConfigureAppConfiguration(x => x.AddUserSecrets(typeof(Program).Assembly))
		.UseOrleansClient((context, client) =>
		{


			var connectionString = context.Configuration["storage"];

			Action<ClusterClientAzureQueueStreamConfigurator> queueConfig = configurator =>
			{
				configurator.ConfigureAzureQueue(
					ob => ob.Configure(options =>
					{
						options.ConfigureQueueServiceClient(connectionString);
						options.QueueNames = new List<string> { "yourprefix-azurequeueprovider-0" };
					}));
				//configurator.ConfigureCacheSize(1024);
				//configurator.ConfigurePullingAgent(ob => ob.Configure(options =>
				//{
				//	options.GetQueueMsgsTimerPeriod = TimeSpan.FromMilliseconds(200);
				//}));
			};


			var hostEntry = Dns.GetHostEntry("silo");
			client.Configure<ClusterOptions>(options =>
							 {
								 options.ClusterId = Constants.ClusterId;
								 options.ServiceId = Constants.ServiceId;

							 });
			client.AddAzureQueueStreams("AzureQueueProvider", queueConfig);
			//client.AddAzureQueueStreams()
			client.UseAzureStorageClustering(options => options.ConfigureTableServiceClient(connectionString));
		})
		.ConfigureLogging(logging => logging.AddConsole());

	var host = builder.Build();
	await host.StartAsync();

	Console.WriteLine("Client successfully connected to silo host \n");

	return host;
}

static async Task DoClientWorkAsync(IClusterClient client)
{
	var id = Guid.Parse("b4d09f63-a45f-485e-a728-91473dd9b46a");

	var vin = "123ABC";

	var streamProvider = client.GetStreamProvider("AzureQueueProvider");

	IAsyncStream<string> chatStream = streamProvider.GetStream<string>("MyNamespace", id);
	await chatStream.OnNextAsync("foo3");

	//var search = client.GetGrain<IVinStoreGrain>("123");
	//await search.RequestSearch();
	////var number = await search.GetNumber();
	//Console.WriteLine($"Number is {number}");
	//await search.AddVin("some_vin");
	//await search.AddMakeId(5);

	var friend = client.GetGrain<IHello>(0);
	var response = await friend.SayHello("Good morning, HelloGrain!");

	Console.WriteLine($"\n\n{response}\n\n");
}