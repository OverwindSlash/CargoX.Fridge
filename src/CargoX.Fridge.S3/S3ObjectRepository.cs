using Abp.IO.Extensions;
using Abp.Reflection.Extensions;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using CargoX.Fridge.Configuration;
using CargoX.Fridge.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CargoX.Fridge.S3
{
    public class S3ObjectRepository : IS3ObjectRepository
    {
        private const string ServiceURLKey = "ObjectStorage:AmazonS3Config:ServiceURL";
        private const string AccessKeyKey = "ObjectStorage:AmazonS3Config:AccessKey";
        private const string SecretKeyKey = "ObjectStorage:AmazonS3Config:SecretKey";

        private readonly string DefaultBucketRegion = "us-east-1";
        private readonly S3CannedACL DefaultCannedAcl = S3CannedACL.PublicRead;
        private readonly ServerSideEncryptionMethod DefaultEncryptionMethod = ServerSideEncryptionMethod.None;

        private readonly ILogger _logger;
        private readonly IConfigurationRoot _configuration;

        private AmazonS3Client _amazonS3Client;

        public S3ObjectRepository(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<S3ObjectRepository>();

            _configuration = AppConfigurations.Get(
                typeof(FridgeS3Module).GetAssembly().GetDirectoryPathOrNull());

            string url = _configuration[ServiceURLKey];
            string accessKey = _configuration[AccessKeyKey];
            string secretKey = _configuration[SecretKeyKey];

            var config = new AmazonS3Config()
            {
                ServiceURL = url,
                ForcePathStyle = true
            };

            _amazonS3Client = new AmazonS3Client(accessKey, secretKey, config);
        }

        public async Task<bool> DoesBucketExistAsync(string bucketName)
        {
            try
            {
                return await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3Client, bucketName.ToLower());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<List<S3Bucket>> ListBucketsAsync()
        {
            try
            {
                ListBucketsResponse response = await _amazonS3Client.ListBucketsAsync();
                return response.HttpStatusCode == HttpStatusCode.OK ? response.Buckets : new List<S3Bucket>();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<bool> CreateBucketAsync(string bucketName)
        {
            PutBucketRequest request = new PutBucketRequest
            {
                BucketName = bucketName.ToLower(),  // Why to lower -> https://github.com/minio/minio/issues/5145
                BucketRegionName = DefaultBucketRegion,
                CannedACL = DefaultCannedAcl
            };

            try
            {
                PutBucketResponse response = await _amazonS3Client.PutBucketAsync(request);
                return (response.HttpStatusCode == HttpStatusCode.OK) ? true : false;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<bool> DeleteBucketAsync(string bucketName)
        {
            DeleteBucketRequest request = new DeleteBucketRequest
            {
                BucketName = bucketName.ToLower(),
            };

            try
            {
                DeleteBucketResponse response = await _amazonS3Client.DeleteBucketAsync(request);
                return (response.HttpStatusCode == HttpStatusCode.NoContent) ? true : false;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<bool> PutObjectWithBytesAsync(string bucketName, string objectKey, byte[] objectBytes)
        {
            if ((objectBytes.Length == 0) || !await DoesBucketExistAsync(bucketName))
            {
                return false;
            }

            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = bucketName.ToLower(),
                CannedACL = DefaultCannedAcl,
                Key = objectKey,
                InputStream = new MemoryStream(objectBytes),
                ServerSideEncryptionMethod = DefaultEncryptionMethod
            };

            try
            {
                PutObjectResponse response = await _amazonS3Client.PutObjectAsync(request);
                return (response.HttpStatusCode == HttpStatusCode.OK) ? true : false;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<bool> PutObjectWithBase64Async(string bucketName, string objectKey, string objectBase64)
        {
            if (string.IsNullOrEmpty(objectBase64))
            {
                return false;
            }

            return await PutObjectWithBytesAsync(bucketName, objectKey, Convert.FromBase64String(objectBase64));
        }

        public async Task<bool> DeleteObjectAsync(string bucketName, string objectKey)
        {
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = bucketName.ToLower(),
                Key = objectKey
            };

            try
            {
                DeleteObjectResponse response = await _amazonS3Client.DeleteObjectAsync(request);
                return (response.HttpStatusCode == HttpStatusCode.NoContent) ? true : false;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<byte[]> GetObjectWithBytesAsync(string bucketName, string objectKey)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName.ToLower(),
                Key = objectKey
            };

            try
            {
                using GetObjectResponse response = await _amazonS3Client.GetObjectAsync(request);
                return response.ResponseStream.GetAllBytes();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<string> GetObjectWithBase64Async(string bucketName, string objectKey)
        {
            byte[] bytes = await GetObjectWithBytesAsync(bucketName, objectKey);
            return Convert.ToBase64String(bytes);
        }

        public async Task<List<S3Object>> ListObjectsAsync(string bucketName, string startKey = "", int maxKeys = 1000)
        {
            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = bucketName.ToLower(),
                StartAfter = startKey,
                MaxKeys = maxKeys
            };

            try
            {
                ListObjectsV2Response response = await _amazonS3Client.ListObjectsV2Async(request);
                return response.S3Objects;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task PurgeBucketAsync(string bucketName)
        {
            await AmazonS3Util.DeleteS3BucketWithObjectsAsync(
                _amazonS3Client,
                bucketName.ToLower(),
                new S3DeleteBucketWithObjectsOptions()
                {
                    ContinueOnError = true,
                    QuietMode = true
                });
        }

        public IClientConfig GetClientConfig()
        {
            return _amazonS3Client.Config;
        }
    }
}

