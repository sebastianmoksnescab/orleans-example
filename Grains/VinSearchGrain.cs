using GrainInterfaces;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Grains
{
	[StorageProvider(ProviderName = "vinSearchStore")]
	[LogConsistencyProvider(ProviderName = "LogStorage")]
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
