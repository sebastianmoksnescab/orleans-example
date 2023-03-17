using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace Grains;

public class HelloGrain : Grain, IHello
{
	private readonly ILogger _logger;

	public HelloGrain(ILogger<HelloGrain> logger) => _logger = logger;

	ValueTask<string> IHello.SayHello(string greeting)
	{
		//Orleans.Streams.IStreamProvider streamProvider = O .GetStreamProvider("MyStreamProvider");



		_logger.LogInformation(
			"SayHello message received: greeting = '{Greeting}'", greeting);

		return ValueTask.FromResult(
			$"""
            Client said: '{greeting}', so HelloGrain says: Hello!
            """);
	}
}