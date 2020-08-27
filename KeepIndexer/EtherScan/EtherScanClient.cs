using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace KeepIndexer.EtherScan
{
	public class EtherScanClient
	{
		string _baseUrl;
		WebClient wc = new WebClient();
		public EtherScanClient(string baseUrl)
		{
			_baseUrl = baseUrl;
		}

		async Task<Response<T>> GetResult<T>(Dictionary<string, object> parameters)
		{
			string requestUrl = _baseUrl + "&" + string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
			string httpApiResult = await wc.DownloadStringTaskAsync(requestUrl);
			return JsonConvert.DeserializeObject<Response<T>>(httpApiResult);
		}

		public async Task<Response<List<Erc20TokenTransfer>>> GetErc20TokenTransfers(string contractaddress, ulong? fromBlock = null, ulong? toBlock = null, string sort = "asc", int? page = 1, int? limit = 1000)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"module", "account" },
				{"action", "tokentx" },
				{"contractaddress", contractaddress },
				{"startblock", fromBlock },
				{"endblock", toBlock ?? 99999999 },
				{"sort", sort },
				{"page", page },
				{"offset",limit }
			};
			return await GetResult<List<Erc20TokenTransfer>>(parameters);
		}
	}
}
