using System;
using System.Collections.Generic;
using System.Linq;
using KeepIndexer.Storage;
using Microsoft.AspNetCore.Mvc;

namespace KeepIndexer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OpController : ControllerBase
	{
		private readonly KeepIndexerContext _context;

		public OpController(KeepIndexerContext context)
		{
			_context = context;
		}

		[HttpGet("list")]
		public ActionResult<IEnumerable<Op>> Ops(string sender)
		{
			var list = _context.Op.ToList();
			return list;
		}

		[HttpGet("txlist")]
		public ActionResult<IEnumerable<string>> TxList(string sender)
		{
			var list = _context.Op.Where(o => o.Sender == sender && o.Contract.Active).Select(o => o.Tx).ToList();
			return list;
		}

		[HttpGet("tdt_id")]
		public ActionResult<string> GetTDT_ID(decimal lot, string token)
		{
			List<string> tdt_list = _context.Op.Where(o => o.Amount == lot && !o.TDTUsed && o.Type == 0 && o.Contract.Active && o.Contract.Token == token).Select(o => o.TDT_ID).ToList();
			if (tdt_list.Count == 0)
				return "";
			return tdt_list[new Random().Next(tdt_list.Count)];
		}
	}
}
