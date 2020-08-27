namespace KeepIndexer.EtherScan
{
	public class Response<T>
	{
		public string status { get; set; }
		public string message { get; set; }
		public T result { get; set; }
	}
}
