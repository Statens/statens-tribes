using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Domain.Infrastructure
{
    public class TribeMembershipListRepository : AzureBlobStorageRepositoryBase<TribeMembershipList>
    {
        public TribeMembershipListRepository(
            IConfiguration configuration, 
            IDistributedCache distributedCache, 
            ILogger<TribeMembershipListRepository> logger) 
            : base(configuration, distributedCache, logger)
        {
        }

        protected override string GetEntityKey(TribeMembershipList entity)
        {
            return entity.GetType().Name + "-" + entity.MemberKey;
        }
    }
}