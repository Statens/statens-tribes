using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Statens.Tribes.App.Domain.Interfaces;

namespace Statens.Tribes.App.Domain.Infrastructure
{
    public abstract class AzureBlobStorageRepositoryBase<T> : IRepositoryOfType<T> where T : class
    {
        private readonly IConfiguration _configuration;
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<AzureBlobStorageRepositoryBase<T>> _logger;

        protected AzureBlobStorageRepositoryBase(
            IConfiguration configuration,
            CloudBlobClient cloudBlobClient,
            IDistributedCache distributedCache,
            ILogger<AzureBlobStorageRepositoryBase<T>> logger)
        {
            _configuration = configuration;
            _cloudBlobClient = cloudBlobClient;
            _distributedCache = distributedCache;
            _logger = logger;

            var container = GetContainerReference(_cloudBlobClient);
            container.CreateIfNotExistsAsync().Wait();

            /*
            var data = GetContainerReference(_cloudBlobClient).ListBlobsSegmentedAsync(null);
            var docs = data.Result.Results.OfType<CloudBlockBlob>();
            foreach(var e in docs)
            {
                GetContainerReference(_cloudBlobClient).GetBlobReference(e.Name).DeleteAsync().Wait();
            }
            */
        }

        protected abstract string GetEntityKey(T entity);

        public async Task<List<T>> ReadAllAsync()
        {
            var data = await GetContainerReference(_cloudBlobClient).ListBlobsSegmentedAsync(null);
            var docs = data.Results.OfType<CloudBlockBlob>().Select(x => x.Name);

            var tasks = docs.Select(ReadAsync);

            var res = await Task.WhenAll(tasks);

            return res.ToList();
        }

        public async Task<T> ReadAsync(string entityKey)
        {
            // var entityKey = typeof(T).Name + "-" + id;

            var cachedContents = await _distributedCache.GetStringAsync(entityKey);
            
            if (!string.IsNullOrEmpty(cachedContents))
            {
                _logger.LogInformation($"Found entityKey {entityKey} in cache");
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cachedContents);
            }

            var blob = GetContainerReference(_cloudBlobClient).GetBlockBlobReference(entityKey);

            var contents = await blob.DownloadTextAsync();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contents);

            if (!string.IsNullOrEmpty(contents))
            {
                _logger.LogInformation($"Write entityKey {entityKey} to cache {contents}");
                await _distributedCache.SetStringAsync(entityKey, contents);
            }

            return data;
        }

        public async Task<T> SaveAsync(T entity)
        {
            var entityKey = GetEntityKey(entity);
            var blob = GetContainerReference(_cloudBlobClient).GetBlockBlobReference(entityKey);

            var contents = Newtonsoft.Json.JsonConvert.SerializeObject(entity);
            await blob.UploadTextAsync(contents);

            if (!string.IsNullOrEmpty(contents))
            {
                _logger.LogInformation($"Write entityKey {entityKey} to cache {contents}");
                await _distributedCache.SetStringAsync(entityKey, contents);
            }

            return await ReadAsync(entityKey);
        }

        private CloudBlobContainer GetContainerReference(CloudBlobClient cloudBlobClient)
        {
            return cloudBlobClient.GetContainerReference(GetType().Name.ToLowerInvariant());
        }
    }
}