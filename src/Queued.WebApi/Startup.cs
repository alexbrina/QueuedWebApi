using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Queued.Domain.Adapters;
using SimpleQueue.Abstractions;
using System;

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
            IServiceProvider serviceProvider, 
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

            // start 3 consumers that will make 3 max attempts to complete work
            queueAdapter.StartConsuming<ISimpleQueueWorker>(
                3, 3, serviceProvider, applicationLifetime.ApplicationStopping);
        }
    }
}
