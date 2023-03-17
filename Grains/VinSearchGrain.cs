using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GrainInterfaces;
using Orleans.Providers;
using Orleans.Runtime;

namespace Grains
{
	[StorageProvider(ProviderName = "vinSearchStore")]
	internal class VinSearchGrain : Grain<VinSearchState>, IVinSearchGrain
	{
		//private readonly IPersistentState<VinSearchState> store;

		//public VinSearchGrain([PersistentState("searches", "vin-search-store")] IPersistentState<VinSearchState> searches)
		//{
		//	this.store = searches;
		//}

		public Task<int> GetNumber()
		{
			return Task.FromResult(State.Number);
		}

		public async Task SetNumber(int number)
		{
			State.Number = number;
			await WriteStateAsync();
		}
	}

	[Serializable]
	public class VinSearchState
	{
		public int Number { get; set; }
	}
}
