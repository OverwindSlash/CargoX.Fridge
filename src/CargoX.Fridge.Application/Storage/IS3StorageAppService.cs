using Abp.Application.Services;
using Amazon.Runtime;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CargoX.Fridge.Storage
{
    public interface IS3StorageAppService : IApplicationService
    {
        public Task<bool> DoesBucketExistAsync(string bucketName);
        public Task<bool> CreateBucketAsync(string bucketName);
        public Task<bool> DeleteBucketAsync(string bucketName);
        public Task<List<S3Bucket>> ListBucketsAsync();
        public Task<bool> PutObjectWithBytesAsync(string bucketName, string objectKey, byte[] objectBytes);
        public Task<bool> PutObjectWithBase64Async(string bucketName, string objectKey, string objectBase64);
        public Task<bool> DeleteObjectAsync(string bucketName, string objectKey);
        public Task<byte[]> GetObjectWithBytesAsync(string bucketName, string objectKey);
        public Task<string> GetObjectWithBase64Async(string bucketName, string objectKey);
        public Task<List<S3Object>> ListObjectsAsync(string bucketName, string startKey = "", int maxKeys = 1000);
        public Task PurgeBucketAsync(string bucketName);
        public IClientConfig GetClientConfig();
    }
}
