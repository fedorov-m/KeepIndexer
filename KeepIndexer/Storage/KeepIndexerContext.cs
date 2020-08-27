using Microsoft.EntityFrameworkCore;

namespace KeepIndexer.Storage
{
	public class KeepIndexerContext : DbContext
	{
		public KeepIndexerContext(DbContextOptions<KeepIndexerContext> options)
			   : base(options)
		{
		}

		public DbSet<Op> Op { get; set; }

		public DbSet<Contract> Contract { get; set; }
	}
}
