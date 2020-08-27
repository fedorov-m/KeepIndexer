using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Nethereum.Web3;
using KeepIndexer.EtherScan;
using KeepIndexer.Storage;

namespace KeepIndexer
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			services.AddDbContext<KeepIndexerContext>(opt => opt.UseSqlite(Configuration.GetConnectionString("Main")));
			services.AddScoped(sp => new Web3(Configuration.GetValue<string>("Web3Url")));
			services.AddScoped(sp => new EtherScanClient(Configuration.GetValue<string>("EtherscanApi")));
			services.AddHostedService<EthWatcher>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseMvc();
		}
	}
}
