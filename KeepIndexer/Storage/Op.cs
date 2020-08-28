using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace KeepIndexer.Storage
{
	public class Op
	{
		[Key]
		public string Tx { get; set; }

		public int Type { get; set; } //0 - deposit, 1 - redeem
		
		public string ContractAddress { get; set; }
		[JsonIgnore]
		public Contract Contract { get; set; }
		public string Sender { get; set; }
		public ulong Block { get; set; }
		public decimal Amount { get; set; }
		public DateTime Timestamp { get; set; }
		public string TDT_ID { get; set; }
		public bool TDTUsed { get; set; }
	}
}
