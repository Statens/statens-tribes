using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Domain.Infrastructure
{
    public class TribeRepository : AzureBlobStorageRepositoryBase<Tribe>
    {
        public TribeRepository(IConfiguration configuration,
            IDistributedCache distributedCache,
            ILogger<TribeRepository> logger)
            : base(configuration, distributedCache, logger)
        {
        }

        protected override string GetEntityKey(Tribe entity)
        {
            return entity.Id;
            // return entity.GetType().Name + "-" + entity.Id;
        }
    }
}