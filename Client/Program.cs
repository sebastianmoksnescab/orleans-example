using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GrainInterfaces;
using Microsoft.Extensions.Configuration;
using Orleans.Configuration;
using System.Net;

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
			var hostEntry = Dns.GetHostEntry("silo");
			client.Configure<ClusterOptions>(options =>
							 {
								 options.ClusterId = "ShoppingCartCluster";
								 options.ServiceId = "ShoppingCartService";
								 
							 });
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
	var friend = client.GetGrain<IHello>(0);
	var response = await friend.SayHello("Good morning, HelloGrain!");

	Console.WriteLine($"\n\n{response}\n\n");
}