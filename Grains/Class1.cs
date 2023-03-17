using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using GrainInterfaces;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Grains
{
	[StorageProvider(ProviderName = "vinSearchStore")]
	[LogConsistencyProvider(ProviderName = "LogStorage")]
	internal class VinStoreGrain : JournaledGrain<VinStoreState>, IVinStoreGrain
	{
		public VinStoreGrain() { }

		public Task<SearchResult> GetSearch()
		{
			SearchResult result;
			switch (State.SearchStatus)
			{
				case SearchStatus.Started:
					result = SearchResult.Started;
					break;
				case SearchStatus.Finished:
					result = SearchResult.Finished;
					break;
				default:
					result = SearchResult.NotStarted;
					break;
			}
			return Task.FromResult(result);
		}

		public Task RequestSearch()
		{
			switch (State.SearchStatus)
			{
				case SearchStatus.NotStarted:
					return StartSearch();
					break;
				case SearchStatus.Finished:
				// Detect if search is old.
				default:
					break;
			}
			return Task.CompletedTask;
		}

		private async Task StartSearch()
		{
			//IStreamProvider streamProvider = base.GetStreamProvider("MyStreamProvider");

			//IAsyncStream<string> chatStream =
			//	streamProvider.GetStream<string>(chatGroupId, "MyNamespace");

			//await chatStream.SubscribeAsync(
			//	async (message, token) => Console.WriteLine(message))
			//RaiseEvent(new VinSearchStarted { });

		}
	}

	[Serializable]
	public class VinStoreState
	{
		public void Apply(VinSearchStarted @event)
		{
			SearchStatus = SearchStatus.Started;
		}

		public SearchStatus SearchStatus { get; private set; }
	}

	public class VinSearchStarted
	{

	}



}
