using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using GrainInterfaces;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Streams;
using Orleans;

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

			var streamProvider = Orleans.GrainStreamingExtensions.GetStreamProvider(this, "AzureQueueProvider");
			//Orleans.Streams.IStreamProvider streamProvider = base.GetStreamProvider("AzureQueueProvider");

			//var guid = new Guid("some guid identifying the chat room");
			//// Get one of the providers which we defined in our config
			//var streamProvider = GetStreamProvider("StreamProvider");
			//// Get the reference to a stream
			var streamId = Orleans.Runtime.StreamId.Create("RANDOMDATA", Guid.NewGuid());
			var stream = streamProvider.GetStream<VinSearchEvent>(streamId);
			await stream.OnNextAsync(new VinSearchEvent { Foo = "bar1" });
			await stream.OnNextAsync(new VinSearchEvent { Foo = "bar2" });
			await stream.OnNextAsync(new VinSearchEvent { Foo = "bar3" });
			await stream.OnNextAsync(new VinSearchEvent { Foo = "bar4" });
			await stream.OnNextAsync(new VinSearchEvent { Foo = "bar5" });
			//IStreamProvider streamProvider = base.GetStreamProvider("MyStreamProvider");

			//IAsyncStream<string> chatStream =
			//	streamProvider.GetStream<string>(chatGroupId, "MyNamespace");

			//await chatStream.SubscribeAsync(
			//	async (message, token) => Console.WriteLine(message))
			//RaiseEvent(new VinSearchStarted { });

		}
	}

	[GenerateSerializer]
	public struct VinSearchEvent
	{
		[Id(0)]
		public string Foo { get; set; }
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
