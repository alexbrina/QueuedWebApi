using Queued.Adapter.Queue;
using Queued.Domain.Adapters;
using SimpleQueue.InMemory;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddQueueAdapter(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSimpleQueue();

            // register with singleton life time
            services.AddSingleton<IQueueAdapter>(provider =>
                new QueueAdapter(new InMemoryQueue(provider))
            );

            return services;
        }
    }
}
