using Queued.Domain.Adapters;
using SimpleQueue.Abstractions;
using SimpleQueue.Abstractions.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Queued.Application
{
    internal class Worker : ISimpleQueueWorker
    {
        private readonly IWorkRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public Worker(IWorkRepository repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository
                ?? throw new ArgumentNullException(nameof(repository));
            this.unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Execute(Work work, CancellationToken cancellationToken)
        {
            await unitOfWork.ExecuteInTransaction(async () =>
            {
                // must update before exection to check status
                await repository.SaveCompleted(work, cancellationToken);

                // just a random exception generator
                var random = new Random();
                if (random.Next(1, 5) == 3)
                {
                    throw new InvalidOperationException("Whatever!");
                }

                // actually do some work
                await Task.Delay((int)(random.NextDouble() * 1000), cancellationToken);
            });
        }
    }
}
