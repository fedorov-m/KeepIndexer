using Nethereum.Util;
using System.Numerics;

namespace KeepIndexer.EtherScan
{
	public class Erc20TokenTransfer
	{
		public ulong blockNumber { get; set; }
		public ulong timeStamp { get; set; }
		public string hash { get; set; }
		public int nonce { get; set; }
		public string blockHash { get; set; }
		public string from { get; set; }
		public string contractAddress { get; set; }
		public string to { get; set; }
		public string tokenName { get; set; }
		public string tokenSymbol { get; set; }
		public int tokenDecimal { get; set; }
		public BigInteger value { get; set; }
		public decimal DecimalValue => (decimal)new BigDecimal(value, tokenDecimal * -1);
		public int transactionIndex { get; set; }
		public decimal gas { get; set; }
		public decimal gasPrice { get; set; }
		public decimal gasUsed { get; set; }
		public decimal cumulativeGasUsed { get; set; }
		public int confirmations { get; set; }
	}
}
