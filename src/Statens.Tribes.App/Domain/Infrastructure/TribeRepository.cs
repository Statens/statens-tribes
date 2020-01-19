using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Domain.Infrastructure
{
    public class TribeRepository : AzureBlobStorageRepositoryBase<Tribe>
    {
        private readonly IConfiguration _configuration;
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<TribeRepository> _logger;

        public TribeRepository(IConfiguration configuration,
            CloudBlobClient cloudBlobClient,
            IDistributedCache distributedCache,
            ILogger<TribeRepository> logger)
            : base(configuration, cloudBlobClient, distributedCache, logger)
        {
            _configuration = configuration;
            _cloudBlobClient = cloudBlobClient;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        protected override string GetEntityKey(Tribe entity)
        {
            return entity.Id;
            // return entity.GetType().Name + "-" + entity.Id;
        }


    }
}