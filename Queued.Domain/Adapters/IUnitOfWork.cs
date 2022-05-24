using System;
using System.Threading.Tasks;

namespace Queued.Domain.Adapters
{
    public interface IUnitOfWork
    {
        Task ExecuteInTransaction(Func<Task> work);

        Task<T> ExecuteInTransaction<T>(Func<Task<T>> work);
    }
}
