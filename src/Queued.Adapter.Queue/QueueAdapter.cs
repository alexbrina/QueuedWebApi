using Microsoft.Extensions.DependencyInjection;
using Queued.Domain.Adapters;
using SimpleQueue.Abstractions;
using SimpleQueue.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Queued.Adapter.Queue
{
    internal class QueueAdapter : IQueueAdapter
    {
        private readonly ISimpleQueue queue;

        public QueueAdapter(ISimpleQueue queue)
        {
            this.queue = queue
                ?? throw new System.ArgumentNullException(nameof(queue));
        }

        public void Add(Work work) => queue.Add(work);

        public void Requeue(IEnumerable<Work> works) => queue.Requeue(works);

        public void StartConsuming<T>(
            int maxAttempts,
            int instances,
            IServiceProvider provider,
            CancellationToken cancellationToken)
            where T : ISimpleQueueWorker
        {
            // each worker and its dependencies (like DbContext that
            // holds a database connection) have its own scope
            for (int i = 1; i <= instances; i++)
            {
                using var scope = provider.CreateScope();
                var worker = scope.ServiceProvider.GetRequiredService<T>();
                queue.Consume(worker, maxAttempts, cancellationToken);
            }
        }
    }
}
