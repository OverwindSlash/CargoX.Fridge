using Abp.Runtime.Caching;
using Amazon.Runtime;
using Amazon.S3.Model;
using CargoX.Fridge.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CargoX.Fridge.Storage
{
    public class S3StorageAppService : IS3StorageAppService
    {
        private readonly IS3ObjectRepository _s3Repository;
        private readonly ICacheManager _cacheManager;


        public S3StorageAppService(
            IS3ObjectRepository s3Repository, 
            ICacheManager cacheManager)
        {
            _s3Repository = s3Repository;
            _cacheManager = cacheManager;
        }

        [HttpGet]
        public async Task<bool> DoesBucketExistAsync(string bucketName)
        {
            return await _s3Repository.DoesBucketExistAsync(bucketName);
        }

        public async Task<bool> CreateBucketAsync(string bucketName)
        {
            return await _s3Repository.CreateBucketAsync(bucketName);
        }

        public async Task<bool> DeleteBucketAsync(string bucketName)
        {
            return await _s3Repository.DeleteBucketAsync(bucketName);
        }

        [HttpGet]
        public async Task<List<S3Bucket>> ListBucketsAsync()
        {
            return await _s3Repository.ListBucketsAsync();
        }

        public async Task<bool> PutObjectWithBytesAsync(string bucketName, string objectKey, byte[] objectBytes)
        {
            return await _s3Repository.PutObjectWithBytesAsync(bucketName, objectKey, objectBytes);
        }

        public async Task<bool> PutObjectWithBase64Async(string bucketName, string objectKey, string objectBase64)
        {
            return await _s3Repository.PutObjectWithBase64Async(bucketName, objectKey, objectBase64);
        }

        public async Task<bool> DeleteObjectAsync(string bucketName, string objectKey)
        {
            return await _s3Repository.DeleteObjectAsync(bucketName, objectKey);
        }

        [HttpGet]
        public async Task<byte[]> GetObjectWithBytesAsync(string bucketName, string objectKey)
        {
            ICache cache = _cacheManager.GetCache("S3Cache");

            string cacheKey = $"{bucketName}:{objectKey}";
            return await cache.GetAsync<string, byte[]>(cacheKey,
                async () => { return await _s3Repository.GetObjectWithBytesAsync(bucketName, objectKey); });
        }

        [HttpGet]
        public async Task<string> GetObjectWithBase64Async(string bucketName, string objectKey)
        {
            return await _s3Repository.GetObjectWithBase64Async(bucketName, objectKey);
        }

        [HttpGet]
        public async Task<List<S3Object>> ListObjectsAsync(string bucketName, string startKey = "", int maxKeys = 1000)
        {
            return await _s3Repository.ListObjectsAsync(bucketName, startKey, maxKeys);
        }

        public async Task PurgeBucketAsync(string bucketName)
        {
            await _s3Repository.PurgeBucketAsync(bucketName);
        }

        [HttpGet]
        public IClientConfig GetClientConfig()
        {
            return _s3Repository.GetClientConfig();
        }
    }
}
