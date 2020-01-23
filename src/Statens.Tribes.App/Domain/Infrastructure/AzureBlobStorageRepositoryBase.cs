using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Statens.Tribes.App.Domain.Interfaces;

namespace Statens.Tribes.App.Domain.Infrastructure
{
    public abstract class AzureBlobStorageRepositoryBase<T> : IRepositoryOfType<T> where T : class
    {
        // private readonly IConfiguration _configuration;
        // private readonly CloudBlobClient _cloudBlobClient;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<AzureBlobStorageRepositoryBase<T>> _logger;
        private readonly BlobServiceClient _blobService;

        protected AzureBlobStorageRepositoryBase(
            IConfiguration configuration,
            // CloudBlobClient cloudBlobClient,
            IDistributedCache distributedCache,
            ILogger<AzureBlobStorageRepositoryBase<T>> logger)
        {
            // _configuration = configuration;
            // _cloudBlobClient = cloudBlobClient;
            _distributedCache = distributedCache;
            _logger = logger;

            //var container = GetContainerReference(_cloudBlobClient);
            //container.CreateIfNotExistsAsync().Wait();

            var azureStorageConnectionString = configuration["AzureStorageConnectionString"];
            
            _blobService = new BlobServiceClient(azureStorageConnectionString);

            GetContainerReference(_blobService).CreateIfNotExistsAsync().Wait();


            // blobService.GetBlobContainerClient("").GetBlobClient()
                
            //var credential = new DefaultAzureCredential();
            //var dd = new BlobContainerClient(new Uri("https://myaccount.blob.core.windows.net/mycontainer"), credential);

            // dd.GetBlobs()
            
            // var blobClient = new BlobClient(new Uri("https://myaccount.blob.core.windows.net/mycontainer/myblob"), credential);

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

        /*
        public async Task<List<T>> ReadAllAsync()
        {
            var data = await GetContainerReference(_cloudBlobClient).ListBlobsSegmentedAsync(null);
            var docs = data.Results.OfType<CloudBlockBlob>().Select(x => x.Name);

            var tasks = docs.Select(ReadAsync);

            var res = await Task.WhenAll(tasks);

            return res.ToList();
        }
        */

        public async Task<List<T>> ReadAllAsync()
        {
            var data = GetContainerReference(_blobService).GetBlobs();
            var docs = data.Select(x => x.Name);

            var tasks = docs.Select(ReadAsync);

            var res = await Task.WhenAll(tasks);

            return res.ToList();
        }

        //public async Task<T> ReadAsync(string entityKey)
        //{
        //    // var entityKey = typeof(T).Name + "-" + id;

        //    var cachedContents = await _distributedCache.GetStringAsync(entityKey);
            
        //    if (!string.IsNullOrEmpty(cachedContents))
        //    {
        //        _logger.LogInformation($"Found entityKey {entityKey} in cache");
        //        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cachedContents);
        //    }

        //    var blob = GetContainerReference(_cloudBlobClient).GetBlockBlobReference(entityKey);

        //    var contents = await blob.DownloadTextAsync();
        //    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contents);

        //    if (!string.IsNullOrEmpty(contents))
        //    {
        //        _logger.LogInformation($"Write entityKey {entityKey} to cache {contents}");
        //        await _distributedCache.SetStringAsync(entityKey, contents);
        //    }

        //    return data;
        //}

        public async Task<T> ReadAsync(string entityKey)
        {
            // var entityKey = typeof(T).Name + "-" + id;

            var cachedContents = await _distributedCache.GetStringAsync(entityKey);

            if (!string.IsNullOrEmpty(cachedContents))
            {
                _logger.LogInformation($"Found entityKey {entityKey} in cache");
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cachedContents);
            }

            var blob = GetContainerReference(_blobService).GetBlobClient(entityKey);

            string contents;
            using (var ms = new MemoryStream())
            {
                await blob.DownloadToAsync(ms);
                contents = Encoding.UTF8.GetString(ms.ToArray());
            }

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contents);

            if (!string.IsNullOrEmpty(contents))
            {
                _logger.LogInformation($"Write entityKey {entityKey} to cache {contents}");
                await _distributedCache.SetStringAsync(entityKey, contents);
            }

            return data;
        }

        //public async Task<T> SaveAsync(T entity)
        //{
        //    var entityKey = GetEntityKey(entity);
        //    var blob = GetContainerReference(_cloudBlobClient).GetBlockBlobReference(entityKey);

        //    var contents = Newtonsoft.Json.JsonConvert.SerializeObject(entity);
        //    await blob.UploadTextAsync(contents);

        //    if (!string.IsNullOrEmpty(contents))
        //    {
        //        _logger.LogInformation($"Write entityKey {entityKey} to cache {contents}");
        //        await _distributedCache.SetStringAsync(entityKey, contents);
        //    }

        //    return await ReadAsync(entityKey);
        //}

        public async Task<T> SaveAsync(T entity)
        {
            var entityKey = GetEntityKey(entity);
            var blob = GetContainerReference(_blobService).GetBlockBlobClient(entityKey);

            var contents = Newtonsoft.Json.JsonConvert.SerializeObject(entity);

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            {
                await blob.UploadAsync(ms);
            }

            if (!string.IsNullOrEmpty(contents))
            {
                _logger.LogInformation($"Write entityKey {entityKey} to cache {contents}");
                await _distributedCache.SetStringAsync(entityKey, contents);
            }

            return await ReadAsync(entityKey);
        }

        //private CloudBlobContainer GetContainerReference(CloudBlobClient cloudBlobClient)
        //{
        //    return cloudBlobClient.GetContainerReference(GetType().Name.ToLowerInvariant());
        //}

        private BlobContainerClient GetContainerReference(BlobServiceClient client)
        {
            return client.GetBlobContainerClient(GetType().Name.ToLowerInvariant());
            // return client.GetBlobContainerClient("BlobContainerClient".ToLowerInvariant() + GetType().Name.ToLowerInvariant());
        }
    }
}