using Queued.Adapter.Storage;
using Queued.Adapter.Storage.Context;
using Queued.Domain.Adapters;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjection
    {
        public static IServiceCollection AddStorageAdapter(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped(services => new DbContext());
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IWorkRepository, WorkRepository>();

            using var context = new DbContext();
            DbSchema.Setup(context);

            return services;
        }
    }
}
