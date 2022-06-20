using Dapper;
using Queued.Adapter.Storage.Context;
using Queued.Domain.Adapters;
using SimpleQueue.Abstractions.Exceptions;
using SimpleQueue.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Queued.Adapter.Storage
{
    internal class WorkRepository : IWorkRepository
    {
        private readonly DbContext context;

        public WorkRepository(DbContext context)
        {
            this.context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task SaveRequested(Work work)
        {
            var cmd = "INSERT INTO Work (Id, Data, RequestedAt) " +
                      "VALUES (@Id, @Data, @RequestedAt)";

            await context.ExecuteAsync(cmd, work);
        }

        public async Task SaveCompleted(Work work, CancellationToken cancellationToken)
        {
            var cmd = @"
                UPDATE Work
                   SET Data = @Data
                     , RequestedAt = @RequestedAt
                     , CompletedAt = @CompletedAt
                     , Attempts = @Attempts
                 WHERE Id = @Id
                   AND CompletedAt IS NULL";

            var affected = await context.ExecuteAsync(cmd, work);

            if (affected == 0)
            {
                cmd = "SELECT CompletedAt FROM Work WHERE Id = @Id";

                var completedAt = await context
                    .ExecuteScalarAsync<string>(cmd, work);

                throw (completedAt == null)
                    ? new WorkIsMissingException(work.Id)
                    : new WorkCompletedException(work.Id, completedAt);
            }
        }

        public async Task<IEnumerable<Work>> GetPending()
        {
            var cmd = "SELECT Id, Data, RequestedAt, Attempts " +
                      "FROM Work WHERE CompletedAt IS NULL";

            var result = await context.Connection.QueryAsync(cmd);

            return result.Select(r =>
                new Work((string)r.Id, (string)r.Data, (string)r.RequestedAt, (int?)r.Attempts));
        }
    }
}
