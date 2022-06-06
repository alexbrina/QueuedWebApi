using SimpleQueue.Abstractions.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Queued.Domain.Adapters
{
    public interface IWorkRepository
    {
        Task SaveRequested(Work work);

        Task SaveCompleted(Work work, CancellationToken cancellationToken);

        Task<IEnumerable<Work>> GetPending();
    }
}
