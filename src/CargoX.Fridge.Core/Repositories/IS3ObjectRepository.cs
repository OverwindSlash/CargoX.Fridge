using Abp.Domain.Repositories;
using Amazon.Runtime;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CargoX.Fridge.Repositories
{
    public interface IS3ObjectRepository : IRepository
    {
        /// <summary>
        /// Determines whether an S3 bucket exists or not. 
        /// </summary>
        /// <param name="bucketName">The name of the bucket to check.</param>
        /// <returns>
        /// False is returned in case S3 responds with a NoSuchBucket error.
        /// True is returned in case of success, AccessDenied error or PermanentRedirect error.
        /// An exception is thrown in case of any other error
        /// </returns>
        public Task<bool> DoesBucketExistAsync(string bucketName);

        /// <summary>
        /// Creates a new bucket.
        /// By default, the bucket is created in use us-east-1 as the Region.
        /// Default Canned ACL is S3CannedACL.PublicRead.
        /// </summary>
        /// <param name="bucketName">The name of the bucket to create.</param>
        /// <returns></returns>
        public Task<bool> CreateBucketAsync(string bucketName);

        /// <summary>
        /// Deletes a exist bucket.
        /// </summary>
        /// <param name="bucketName">The name of the bucket to delete.</param>
        /// <returns></returns>
        public Task<bool> DeleteBucketAsync(string bucketName);

        /// <summary>
        /// Returns a list of all buckets owned by the authenticated sender of the request.
        /// </summary>
        /// <returns>The response from the ListBuckets service method, as returned by S3.</returns>
        public Task<List<S3Bucket>> ListBucketsAsync();

        /// <summary>
        /// Uploads object using byte array.
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectKey"></param>
        /// <param name="objectBytes"></param>
        /// <returns></returns>
        public Task<bool> PutObjectWithBytesAsync(string bucketName, string objectKey, byte[] objectBytes);

        /// <summary>
        /// Uploads object using base64 string.
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectKey"></param>
        /// <param name="objectBase64"></param>
        /// <returns></returns>
        public Task<bool> PutObjectWithBase64Async(string bucketName, string objectKey, string objectBase64);

        /// <summary>
        /// Removes the null version (if there is one) of an object and inserts a delete marker,
        /// which becomes the latest version of the object.
        /// If there isn't a null version, Amazon S3 does not remove any objects.
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectKey"></param>
        /// <returns>The response from the DeleteObject service method, as returned by S3.</returns>
        public Task<bool> DeleteObjectAsync(string bucketName, string objectKey);

        /// <summary>
        /// Downloads object using byte array.
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        public Task<byte[]> GetObjectWithBytesAsync(string bucketName, string objectKey);

        /// <summary>
        /// Downloads object using base64 string.
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        public Task<string> GetObjectWithBase64Async(string bucketName, string objectKey);

        /// <summary>
        /// Returns some or all (up to 1,000) of the objects in a bucket.
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="startKey"></param>
        /// <param name="maxKeys"></param>
        /// <returns></returns>
        public Task<List<S3Object>> ListObjectsAsync(string bucketName, string startKey = "", int maxKeys = 1000);

        /// <summary>
        /// Deletes bucket and all objects in it.
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public Task PurgeBucketAsync(string bucketName);

        /// <summary>
        /// Get client configuration.
        /// </summary>
        /// <returns></returns>
        public IClientConfig GetClientConfig();
    }
}
