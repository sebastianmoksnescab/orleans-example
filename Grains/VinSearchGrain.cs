using GrainInterfaces;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;

namespace Grains
{
	[StorageProvider(ProviderName = "vinSearchStore")]
	[LogConsistencyProvider(ProviderName = "LogStorage")]
	[ImplicitStreamSubscription("MyNamespace")]
	internal class VinSearchGrain : JournaledGrain<VinSearchState>, IVinSearchGrain
	{
		public async Task AddVin(string vin)
		{
			RaiseEvent(new VinAddedEvent()
			{
				Vin = vin
			});
			var state = State;
			await ConfirmEvents();
		}

		public async Task AddMakeId(int makeId)
		{
			RaiseEvent(new MakeIdAddedEvent()
			{
				MakeId = makeId
			});
			var state = State;
			await ConfirmEvents();
		}

		public override async Task OnActivateAsync(CancellationToken cancellationToken)
		{
			// Create a GUID based on our GUID as a grain
			var guid = this.GetPrimaryKey();

			// Get one of the providers which we defined in config
			var streamProvider = this.GetStreamProvider("AzureQueueProvider");

			// Get the reference to a stream
			var streamId = StreamId.Create("MyNamespace", guid);
			var stream = streamProvider.GetStream<string>(streamId);

			// Set our OnNext method to the lambda which simply prints the data.
			// This doesn't make new subscriptions, because we are using implicit 
			// subscriptions via [ImplicitStreamSubscription].
			await stream.SubscribeAsync<string>(
				async (data, token) =>
				{
					Console.WriteLine(data);
					await Task.CompletedTask;
				});

			await base.OnActivateAsync(cancellationToken);
		}
	}

	[Serializable]
	public class VinSearchState
	{
		public void Apply(VinAddedEvent @event)
		{
			Vin = @event.Vin;
		}

		public void Apply(MakeIdAddedEvent @event)
		{
			MakeId = @event.MakeId;
		}

		public string Vin { get; private set; }
		public int MakeId { get; private set; }
	}

	[Serializable]
	public class VinAddedEvent
	{
		public string Vin { get; set; }
	}

	[Serializable]
	public class MakeIdAddedEvent
	{
		public int MakeId { get; set; }
	}

}
