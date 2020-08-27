using System.ComponentModel.DataAnnotations;

namespace KeepIndexer.Storage
{
	public class Contract
	{
		[Key]
		public string Address { get; set; }

		public bool Active { get; set; }
		public string Token { get; set; }
		public ulong LastBlock { get; set; }
		public string EncodedAddress => "0x000000000000000000000000" + Address.Substring(2);
	}
}
