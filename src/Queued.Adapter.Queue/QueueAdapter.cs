using Queued.Domain.Adapters;
using SimpleQueue.Abstractions;
using SimpleQueue.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Queued.Adapter.Queue
{
    internal sealed class QueueAdapter : IQueueAdapter
    {
        private readonly ISimpleQueue queue;

        public QueueAdapter(ISimpleQueue queue)
        {
            this.queue = queue
                ?? throw new ArgumentNullException(nameof(queue));
        }

        public void Add(Work work) => queue.Add(work);

        public void Requeue(IEnumerable<Work> works) => queue.Requeue(works);

        public void StartConsuming<T>(
            int maxAttempts,
            int instances,
            CancellationToken cancellationToken)
            where T : ISimpleQueueWorker
        {
            for (int i = 1; i <= instances; i++)
            {
                queue.Consume<T>(maxAttempts, null, cancellationToken);
            }
        }
    }
}
