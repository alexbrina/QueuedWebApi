using Queued.Domain.Adapters;
using System;
using System.Threading.Tasks;

namespace Queued.Adapter.Storage.Context
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext context;

        public UnitOfWork(DbContext context)
        {
            this.context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task ExecuteInTransaction(Func<Task> work)
        {
            using var trans = context.BeginTransaction();
            await work();
            trans.Commit();
        }

        public async Task<T> ExecuteInTransaction<T>(Func<Task<T>> work)
        {
            using var trans = context.BeginTransaction();
            var result = await work();
            trans.Commit();
            return result;
        }
    }
}
