namespace GrainInterfaces
{
	public interface IVinSearchGrain : IGrainWithGuidKey
	{
		Task AddVin(string vin);
		Task AddMakeId(int makeId);
	}

	public interface IVinStoreGrain : IGrainWithStringKey
	{
		Task RequestSearch();
		Task<SearchResult> GetSearch();
	}

	public class SearchResult
	{
		private SearchResult(SearchStatus status)
		{
			Status = status;
		}
		public SearchStatus Status { get; private set; }

		public static SearchResult Started => new SearchResult(SearchStatus.Started);
		public static SearchResult Finished => new SearchResult(SearchStatus.Finished);
		public static SearchResult NotStarted => new SearchResult(SearchStatus.NotStarted);
	}

	public enum SearchStatus
	{
		NotStarted,
		Started,
		Finished
	}

}
