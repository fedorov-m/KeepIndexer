using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KeepIndexer.EtherScan;
using Newtonsoft.Json;
using KeepIndexer.Storage;

namespace KeepIndexer
{
	public class EthWatcher : IHostedService, IDisposable
	{
		private const string TRANSFER_EVENT_CODE = "0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef";

		private readonly ILogger<EthWatcher> _logger;
		private Timer _timer;
		private IServiceProvider _services;

		public EthWatcher(ILogger<EthWatcher> logger, IServiceProvider services)
		{
			_logger = logger;
			_services = services;
			using (var scope = _services.CreateScope())
			{
				using (var context = scope.ServiceProvider.GetService<KeepIndexerContext>())
					context.Database.Migrate();
			}
		}

		public Task StartAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("EthWatcher Service running.");

			_timer = new Timer(DoWork, null, TimeSpan.Zero,
				TimeSpan.FromSeconds(15));

			return Task.CompletedTask;
		}

		private async void DoWork(object state)
		{
			try
			{
				_timer.Change(Timeout.Infinite, Timeout.Infinite);
				_logger.LogInformation("EthWatcher is working");
				const string burnerContract = "0x0000000000000000000000000000000000000000";

				using (var scope = _services.CreateScope())
				{
					using (var context = scope.ServiceProvider.GetService<Storage.KeepIndexerContext>())
					{
						var contract = context.Contract.FirstOrDefault(o => o.Active);
						if (contract != null)
						{
							var web3 = scope.ServiceProvider.GetService<Web3>();
							var etherScan = scope.ServiceProvider.GetService<EtherScanClient>();
							HexBigInteger toBlockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
							if (contract.LastBlock == toBlockNumber.ToUlong())
								return;

							var result = await etherScan.GetErc20TokenTransfers(contract.Address, contract.LastBlock + 1, toBlockNumber.ToUlong(), limit: 5000);
							foreach (var transfer in result.result)
							{
								if (transfer.value == 0 || context.Op.Any(o => o.Tx == transfer.hash))
									continue;
								if (transfer.from == burnerContract ||
									transfer.to == burnerContract)
								{
									var transaction = web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transfer.hash).ConfigureAwait(true).GetAwaiter().GetResult();

									string tdt_id = "0x" + transaction.Input.Substring(34, 40);

									var op = new Storage.Op
									{
										Tx = transfer.hash,
										Block = transfer.blockNumber,
										Contract = contract,
										Sender = transaction.From.ToLower(),
										Amount = result.result.Where(o => o.hash == transfer.hash).Sum(o => o.DecimalValue),
										TDT_ID = tdt_id,
										Timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(transfer.timeStamp),
										Type = transfer.from == burnerContract ? 0 : 1,
										TDTUsed = transfer.to == burnerContract
									};
									context.Add(op);
									if (op.Type == 1)
									{
										var deposit = context.Op.FirstOrDefault(o => o.TDT_ID == op.TDT_ID && o.Type == 0);
										deposit.TDTUsed = true;
									}
									contract.LastBlock = op.Block;
									_logger.LogInformation(JsonConvert.SerializeObject(op));
									context.SaveChanges();
								}
							}
							if (result.result.Count == 5000)
								contract.LastBlock = result.result.Last().blockNumber;
							else
								contract.LastBlock = toBlockNumber.ToUlong();
							context.SaveChanges();
						}
					}
				}

			}
			catch(Exception e)
			{
				_logger.LogError(e, e.Message);
			}
			finally
			{
				_timer.Change(TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15));
			}
		}

		public Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("EthWatcher is stopping.");

			_timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}
}
