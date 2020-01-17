using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Domain.Interfaces
{
    public interface ITribeRepository
    {
        Task<Tribe> SaveAsync(Tribe tribe);

        Task<List<Tribe>> ReadAllAsync();

        Task<Tribe> ReadAsync(string id);
    }

    public class TribeRepository : ITribeRepository
    {
        private readonly IConfiguration _configuration;
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<TribeRepository> _logger;

        public TribeRepository(IConfiguration configuration,
                               CloudBlobClient cloudBlobClient,
                               IDistributedCache distributedCache,
                               ILogger<TribeRepository> logger)
        {
            _configuration = configuration;
            _cloudBlobClient = cloudBlobClient;
            _distributedCache = distributedCache;
            _logger = logger;

            var container = GetTribeContainerReference(_cloudBlobClient);
            container.CreateIfNotExistsAsync().Wait();
        }

        private CloudBlobContainer GetTribeContainerReference(CloudBlobClient cloudBlobClient)
        {
            return cloudBlobClient.GetContainerReference(GetType().Name.ToLowerInvariant());
        }

        public async Task<Tribe> SaveAsync(Tribe tribe)
        {
            var cacheKey = typeof(Tribe).Name + "-" + tribe.Id;
            var contents = Newtonsoft.Json.JsonConvert.SerializeObject(tribe);
            var blob = GetTribeContainerReference(_cloudBlobClient).GetBlockBlobReference(tribe.Id);

            await blob.UploadTextAsync(contents);

            if (!string.IsNullOrEmpty(contents))
            {
                await _distributedCache.SetStringAsync(cacheKey, contents);
            }

            return await ReadAsync(tribe.Id);
        }

        public async Task<List<Tribe>> ReadAllAsync()
        {
            BlobContinuationToken token = null;
            var data = GetTribeContainerReference(_cloudBlobClient).ListBlobsSegmentedAsync(token);
            var docs = data.Result.Results.OfType<CloudBlockBlob>().Select(x => x.Name);

            var tasks = docs.Select(ReadAsync);

            var res = await Task.WhenAll(tasks);

            return res.ToList();
        }
        
        public async Task<Tribe> ReadAsync(string id)
        {
            var cacheKey = typeof(Tribe).Name + "-" + id;
            var cachedContents = await _distributedCache.GetStringAsync(cacheKey);
            if(!string.IsNullOrEmpty(cachedContents))
            {
                _logger.LogInformation($"Found id {cacheKey} cache");
                _logger.LogInformation(cachedContents);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Tribe>(cachedContents);
            }

            var blob = GetTribeContainerReference(_cloudBlobClient).GetBlockBlobReference(id);

            var contents = await blob.DownloadTextAsync();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Tribe>(contents);

            if(!string.IsNullOrEmpty(contents))
            {
                await _distributedCache.SetStringAsync(cacheKey, contents);
            }

            return data;
        }
    }
}