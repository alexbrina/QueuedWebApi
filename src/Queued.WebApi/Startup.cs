using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Queued.Domain.Adapters;
using SimpleQueue.Abstractions;

namespace Queued.WebApi
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
            services.AddWebApi();
            services.AddApplication();
            services.AddStorageAdapter();
            services.AddQueueAdapter();
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IQueueAdapter queueAdapter,
            IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "Queued.WebApi v1"));
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // there will be 3 max attempts to complete work by 3 consumer instances
            queueAdapter.StartConsuming<ISimpleQueueWorker>(
                3, 3, applicationLifetime.ApplicationStopping);
        }
    }
}
