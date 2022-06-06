using SimpleQueue.Abstractions;
using SimpleQueue.InMemory;
using System;
using System.Threading;

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

            // register with singleton life time
            services.AddSingleton<ISimpleQueue>(provider =>
            {
                var queue = new InMemoryQueue(provider);

                // each worker and its dependencies (like DbContext that
                // holds a database connection) have its own scope
                for (int i = 1; i < 4; i++)
                {
                    using var scope = provider.CreateScope();
                    var worker = scope.ServiceProvider.GetRequiredService<ISimpleQueueWorker>();
                    queue.Consume(worker, 3, CancellationToken.None);
                }

                return queue;
            });

            services.AddSimpleQueue();

            return services;
        }
    }
}
