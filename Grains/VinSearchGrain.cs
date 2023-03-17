using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GrainInterfaces;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Runtime;
using static System.Net.Mime.MediaTypeNames;

namespace Grains
{
	[StorageProvider(ProviderName = "vinSearchStore")]
	internal class VinSearchGrain : JournaledGrain<VinSearchState>, IVinSearchGrain
	{
		//private readonly IPersistentState<VinSearchState> store;

		//public VinSearchGrain([PersistentState("searches", "vin-search-store")] IPersistentState<VinSearchState> searches)
		//{
		//	this.store = searches;
		//}

		//public Task<int> GetNumber()
		//{
		//	return Task.FromResult(State.Number);
		//}

		//public async Task SetNumber(int number)
		//{
		//	State.Number = number;
		//	await WriteStateAsync();
		//}

		public Task AddVin(string vin)
		{
			RaiseEvent(new VinAddedEvent()
			{
				Vin = vin
			});
			var state = State;
			return Task.CompletedTask;
		}

		public Task AddMakeId(int makeId)
		{
			RaiseEvent(new MakeIdAddedEvent()
			{
				MakeId = makeId
			});
			var state = State;
			return Task.CompletedTask;
		}
	}

	[Serializable]
	public class VinSearchState
	{
		void Apply(VinAddedEvent @event)
		{
			// code that updates the state
			Vin = @event.Vin;
		}

		void Apply(MakeIdAddedEvent @event)
		{
			// code that updates the state
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
