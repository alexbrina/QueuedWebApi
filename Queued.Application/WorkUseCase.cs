using Microsoft.Extensions.Logging;
using Queued.Domain.Adapters;
using SimpleQueue.Abstractions;
using SimpleQueue.Abstractions.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Queued.Application
{
    public interface IWorkUseCase
    {
        Task Add(WorkRequest request);
        Task Requeue();
    }

    internal class WorkUseCase : IWorkUseCase
    {
        private readonly IWorkRepository workRepository;
        private readonly ISimpleQueue queue;
        private readonly ILogger<WorkUseCase> logger;

        private static string Id => $"{DateTime.Now:yyMMddHHmmss}" +
            $"{Interlocked.Increment(ref sequence):00000000}";
        private static int sequence = 0;

        public WorkUseCase(
            IWorkRepository workRepository,
            ISimpleQueue queue,
            ILogger<WorkUseCase> logger)
        {
            this.workRepository = workRepository
                ?? throw new ArgumentNullException(nameof(workRepository));
            this.queue = queue
                ?? throw new ArgumentNullException(nameof(queue));
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Add(WorkRequest request)
        {
            var work = new Work(Id, request.Data);
            try
            {
                await workRepository.SaveRequested(work);
                queue.Add(work);
            }
            catch
            {
                logger.LogError($"Error trying to save requested work " +
                    $"{work.Id} with data {work.Data}");
                throw;
            }
        }

        public async Task Requeue()
        {
            try
            {
                var pending = await workRepository.GetPending();
                queue.Requeue(pending);
            }
            catch
            {
                logger.LogError($"Error trying to retrieve pending work");
                throw;
            }
        }
    }
}
