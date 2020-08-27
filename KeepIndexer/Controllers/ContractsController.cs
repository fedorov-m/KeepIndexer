using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.Contracts;
using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;
using KeepIndexer.EtherScan;
using KeepIndexer.Storage;

namespace KeepIndexer.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
		readonly KeepIndexerContext _context;
		readonly EtherScanClient _etherScanClient;

        public ContractsController(KeepIndexerContext context, EtherScanClient etherScanClient)
        {
            _context = context;
			_etherScanClient = etherScanClient;
        }

        [HttpGet("list")]
        public IEnumerable<Storage.Contract> GetContract()
        {
            return _context.Contract;
        }

		[HttpGet("add")]
		public async Task<Storage.Contract> Add(string address)
		{
			address = address.ToLower();
			var token = await _etherScanClient.GetErc20TokenTransfers(address, limit: 1);

			var contracts = await _context.Contract.ToListAsync();
			contracts.ForEach(o => o.Active = false);

			var contract = contracts.FirstOrDefault(o => o.Address == address);
			if (contract == null)
			{
				contract = new Storage.Contract { Address = address, LastBlock = token.result[0].blockNumber - 1 };
				_context.Add(contract);
			}
			contract.Active = true;
			contract.Token = token.result[0].tokenSymbol;
			await _context.SaveChangesAsync();
			return contract;
		}

		private static string DecodeOutput(string output, Function function)
		{
			var outputBytes = output.HexToByteArray();
			if (outputBytes.Length <= 32)
				return Encoding.UTF8.GetString(outputBytes, 0, outputBytes.Length).Replace("\0", "").Trim();

			return function.DecodeSimpleTypeOutput<string>(output);
		}
	}
}