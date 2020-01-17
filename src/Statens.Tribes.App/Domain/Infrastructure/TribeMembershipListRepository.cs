using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Statens.Tribes.App.Domain.Interfaces;
using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Domain.Infrastructure
{
    public class TribeMembershipListRepository : AzureBlobStorageRepositoryBase<TribeMembershipList>
    {
        public TribeMembershipListRepository(
            IConfiguration configuration, 
            CloudBlobClient cloudBlobClient, 
            IDistributedCache distributedCache, 
            ILogger<TribeMembershipListRepository> logger) 
            : base(configuration, cloudBlobClient, distributedCache, logger)
        {
        }

        protected override string GetEntityKey(TribeMembershipList entity)
        {
            return entity.GetType().Name + "-" + entity.MemberKey;
        }
    }
}