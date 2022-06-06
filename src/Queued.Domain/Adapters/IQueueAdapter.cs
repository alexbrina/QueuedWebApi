﻿using SimpleQueue.Abstractions;
using SimpleQueue.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Queued.Domain.Adapters
{
    public interface IQueueAdapter
    {
        void Add(Work work);

        void Requeue(IEnumerable<Work> works);

        void StartConsuming<T>(
            int maxAttempts,
            int instances,
            IServiceProvider provider,
            CancellationToken cancellationToken)
            where T : ISimpleQueueWorker;
    }
}
