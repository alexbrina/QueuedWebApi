using Queued.Application;
using SimpleQueue.Abstractions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<IWorkUseCase, WorkUseCase>();
            services.AddScoped<ISimpleQueueWorker, Worker>();

            return services;
        }
    }
}
