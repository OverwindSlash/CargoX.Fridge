using Amazon.S3;
using Amazon.S3.Model;
using CargoX.Fridge.Repositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace CargoX.Fridge.Tests.S3Repository
{
    public class S3ObjectRepository_Tests : FridgeTestBase
    {
        private readonly IS3ObjectRepository _s3Repository;

        public S3ObjectRepository_Tests()
        {
            _s3Repository = Resolve<IS3ObjectRepository>();
        }

        [Fact]
        public async Task TestDoesBucketExistAsync()
        {
            string bucketName = "test-bucket-exist";

            List<S3Bucket> buckets = await _s3Repository.ListBucketsAsync();
            bool bucketExist = buckets.Exists(b => b.BucketName == bucketName);

            bool result = await _s3Repository.DoesBucketExistAsync(bucketName);
            result.ShouldBe(bucketExist);
        }

        [Fact]
        public async Task TestCreateBucketAsyncWhenNotExist()
        {
            string bucketName = "test-create-bucket";

            // Prepare
            bool isBucketExist = await _s3Repository.DoesBucketExistAsync(bucketName);
            if (isBucketExist)
            {
                bool removeResult = await _s3Repository.DeleteBucketAsync(bucketName);
                removeResult.ShouldBe(true);
            }

            // Test
            bool result = await _s3Repository.CreateBucketAsync(bucketName);
            result.ShouldBe(true);

            // Clean up
            bool cleanupResult = await _s3Repository.DeleteBucketAsync(bucketName);
            cleanupResult.ShouldBe(true);
        }

        [Fact]
        public async Task TestCreateBucketAsyncWhenExist()
        {
            string bucketName = "test-create-bucket";

            // Prepare
            bool isBucketExist = await _s3Repository.DoesBucketExistAsync(bucketName);
            if (!isBucketExist)
            {
                bool createResult = await _s3Repository.CreateBucketAsync(bucketName);
                createResult.ShouldBe(true);
            }

            try
            {
                // Test
                bool result = await _s3Repository.CreateBucketAsync(bucketName);
            }
            catch (Exception e)
            {
                e.GetType().ShouldBe(typeof(AmazonS3Exception));
                ((AmazonS3Exception) e).ErrorCode.ShouldBe("BucketAlreadyOwnedByYou");
            }
            finally
            {
                // Clean up
                bool cleanupResult = await _s3Repository.DeleteBucketAsync(bucketName);
                cleanupResult.ShouldBe(true);
            }
        }

        [Fact]
        public async Task TestDeleteBucketAsyncWhenExist()
        {
            string bucketName = "test-delete-bucket";

            // Prepare
            bool isBucketExist = await _s3Repository.DoesBucketExistAsync(bucketName);

            if (!isBucketExist)
            {
                bool createResult = await _s3Repository.CreateBucketAsync(bucketName);
                createResult.ShouldBe(true);
            }

            // Test
            bool deleteResult = await _s3Repository.DeleteBucketAsync(bucketName);
            deleteResult.ShouldBe(true);
        }

        [Fact]
        public async Task TestDeleteBucketAsyncWhenNotExist()
        {
            string bucketName = "test-delete-bucket";

            // Prepare
            bool isBucketExist = await _s3Repository.DoesBucketExistAsync(bucketName);

            if (isBucketExist)
            {
                bool deleteResult = await _s3Repository.DeleteBucketAsync(bucketName);
                deleteResult.ShouldBe(true);
            }

            try
            {
                // Test
                bool result = await _s3Repository.DeleteBucketAsync(bucketName);
                result.ShouldBe(true);
            }
            catch (Exception e)
            {
                e.GetType().ShouldBe(typeof(AmazonS3Exception));
                ((AmazonS3Exception)e).ErrorCode.ShouldBe("NoSuchBucket");
            }
        }

        [Fact]
        public async Task TestListBucketsAsync()
        {
            string bucketName = "test-list-bucket";

            // Prepare
            bool createBucketResult = await _s3Repository.CreateBucketAsync(bucketName);
            createBucketResult.ShouldBe(true);

            // Test
            List<S3Bucket> buckets = await _s3Repository.ListBucketsAsync();
            buckets.Count.ShouldBeGreaterThan(0);

            // Clean up
            bool removeBucketResult = await _s3Repository.DeleteBucketAsync(bucketName);
            removeBucketResult.ShouldBe(true);
        }

        [Fact]
        public async Task TestPutObjectWithBytesAsync()
        {
            string bucketName = "test-put-object";
            string objectKey = "image.jpg";

            // Prepare
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       bool createBucketResult = await _s3Repository.CreateBucketAsync(bucketName);
            createBucketResult.ShouldBe(true);

            // Test
            byte[] bytes = await File.ReadAllBytesAsync("images/image.jpg");
            bool putObjResult = await _s3Repository.PutObjectWithBytesAsync(bucketName, objectKey, bytes);
            putObjResult.ShouldBe(true);

            byte[] getBytes = await _s3Repository.GetObjectWithBytesAsync(bucketName, objectKey);
            bytes.Length.ShouldBe(getBytes.Length);

            // Clean up
            bool deleteObjResult = await _s3Repository.DeleteObjectAsync(bucketName, objectKey);
            deleteObjResult.ShouldBe(true);

            bool deleteBucketResult = await _s3Repository.DeleteBucketAsync(bucketName);
            deleteBucketResult.ShouldBe(true);
        }

        [Fact]
        public async Task TestPutObjectWithBase64Async()
        {
            string bucketName = "test-put-object";
            string objectKey = "image.jpg";

            // Prepare
            bool createBucketResult = await _s3Repository.CreateBucketAsync(bucketName);
            createBucketResult.ShouldBe(true);

            // Test
            byte[] bytes = await File.ReadAllBytesAsync("images/image.jpg");
            bool putObjResult = await _s3Repository.PutObjectWithBase64Async(bucketName, objectKey, Convert.ToBase64String(bytes));
            putObjResult.ShouldBe(true);

            string contents = await _s3Repository.GetObjectWithBase64Async(bucketName, objectKey);
            bytes.Length.ShouldBe(Convert.FromBase64String(contents).Length);

            // Clean up
            bool deleteObjResult = await _s3Repository.DeleteObjectAsync(bucketName, objectKey);
            deleteObjResult.ShouldBe(true);

            bool deleteBucketResult = await _s3Repository.DeleteBucketAsync(bucketName);
            deleteBucketResult.ShouldBe(true);
        }

        [Fact]
        public async Task TestPutObjectWithBytesAsyncWhenBucketNotExist()
        {
            string bucketName = "test-put-object";
            string objectKey = "image.jpg";

            // Prepare
            bool createBucketResult = await _s3Repository.CreateBucketAsync(bucketName);
            createBucketResult.ShouldBe(true);

            // Test
            byte[] bytes = await File.ReadAllBytesAsync("images/image.jpg");
            bool putObjResult = await _s3Repository.PutObjectWithBytesAsync("not-exist", objectKey, bytes);
            putObjResult.ShouldBe(false);

            // Clean up
            bool deleteBucketResult = await _s3Repository.DeleteBucketAsync(bucketName);
            deleteBucketResult.ShouldBe(true);
        }

        [Fact]
        public async Task TestDeleteObjectAsync()
        {
            string bucketName = "test-delete-object";
            string objectKey = "image.jpg";

            // Prepare
            bool createBucketResult = await _s3Repository.CreateBucketAsync(bucketName);
            createBucketResult.ShouldBe(true);

            byte[] bytes = await File.ReadAllBytesAsync("images/image.jpg");
            bool putObjResult = await _s3Repository.PutObjectWithBytesAsync(bucketName, objectKey, bytes);
            putObjResult.ShouldBe(true);

            byte[] getBytes = await _s3Repository.GetObjectWithBytesAsync(bucketName, objectKey);
            bytes.Length.ShouldBe(getBytes.Length);

            // Test
            bool deleteObjResult = await _s3Repository.DeleteObjectAsync(bucketName, objectKey);
            deleteObjResult.ShouldBe(true);

            // Clean up
            bool deleteBucketResult = await _s3Repository.DeleteBucketAsync(bucketName);
            deleteBucketResult.ShouldBe(true);
        }

        [Fact]
        public async Task TestDeleteObjectAsyncWithWrongKey()
        {
            string bucketName = "test-delete-object";
            string objectKey = "image.jpg";

            // Prepare
            bool createBucketResult = await _s3Repository.CreateBucketAsync(bucketName);
            createBucketResult.ShouldBe(true);

            byte[] bytes = await File.ReadAllBytesAsync("images/image.jpg");
            bool putObjResult = await _s3Repository.PutObjectWithBytesAsync(bucketName, objectKey, bytes);
            putObjResult.ShouldBe(true);

            byte[] getBytes = await _s3Repository.GetObjectWithBytesAsync(bucketName, objectKey);
            bytes.Length.ShouldBe(getBytes.Length);

            // Test
            bool deleteObjResult = await _s3Repository.DeleteObjectAsync(bucketName, "wrong-key");
            deleteObjResult.ShouldBe(true); // delete wrong key does not report false result.

            // Clean up
            bool cleanupResult = await _s3Repository.DeleteObjectAsync(bucketName, objectKey);
            cleanupResult.ShouldBe(true);

            bool deleteBucketResult = await _s3Repository.DeleteBucketAsync(bucketName);
            deleteBucketResult.ShouldBe(true);
        }

        [Fact]
        public async Task TestDeleteObjectAsyncWithWrongBucket()
        {
            string bucketName = "test-delete-object";
            string objectKey = "image.jpg";

            // Prepare
            bool createBucketResult = await _s3Repository.CreateBucketAsync(bucketName);
            createBucketResult.ShouldBe(true);

            byte[] bytes = await File.ReadAllBytesAsync("images/image.jpg");
            bool putObjResult = await _s3Repository.PutObjectWithBytesAsync(bucketName, objectKey, bytes);
            putObjResult.ShouldBe(true);

            byte[] getBytes = await _s3Repository.GetObjectWithBytesAsync(bucketName, objectKey);
            bytes.Length.ShouldBe(getBytes.Length);

            // Test
            try
            {
                bool deleteObjResult = await _s3Repository.DeleteObjectAsync("wrong-bucket", objectKey);
                deleteObjResult.ShouldBe(true); // delete wrong key does not report false result.
            }
            catch (Exception e)
            {
                e.GetType().ShouldBe(typeof(AmazonS3Exception));
                ((AmazonS3Exception)e).ErrorCode.ShouldBe("NoSuchBucket");
            }

            // Clean up
            bool cleanupResult = await _s3Repository.DeleteObjectAsync(bucketName, objectKey);
            cleanupResult.ShouldBe(true);

            bool deleteBucketResult = await _s3Repository.DeleteBucketAsync(bucketName);
            deleteBucketResult.ShouldBe(true);
        }

        [Fact]
        public async Task TestListObjectsAsyncWithOutPaging()
        {
            string bucketName = "test-list-object";
            int maxNum = 76;

            // Prepare
            bool createBucketResult = await _s3Repository.CreateBucketAsync(bucketName);
            createBucketResult.ShouldBe(true);

            for (int i = 0; i < maxNum; i++)
            {
                await _s3Repository.PutObjectWithBytesAsync(
                    bucketName.ToLower(),
                    i.ToString(),
                    Convert.FromBase64String("dGVzdGNvbnRlbnQ="));
            }
            
            // Test
            List<S3Object> objects = await _s3Repository.ListObjectsAsync(bucketName);
            objects.Count.ShouldBe(maxNum);

            // Clean up
            await _s3Repository.PurgeBucketAsync(bucketName);
        }

        [Fact]
        public async Task TestListObjectsAsyncWithPaging()
        {
            string bucketName = "test-list-object";
            int maxNum = 50;

            // Prepare
            bool createBucketResult = await _s3Repository.CreateBucketAsync(bucketName);
            createBucketResult.ShouldBe(true);

            for (int i = 0; i < maxNum; i++)
            {
                await _s3Repository.PutObjectWithBytesAsync(
                    bucketName.ToLower(),
                    i.ToString(),
                    Convert.FromBase64String("dGVzdGNvbnRlbnQ="));
            }

            // Test
            List<S3Object> objects = await _s3Repository.ListObjectsAsync(bucketName, "10", 10);
            objects.Count.ShouldBe(10);

            // Clean up
            await _s3Repository.PurgeBucketAsync(bucketName);
        }

        [Fact]
        public async Task TestGetObjectWithBytesAsync()
        {
            string bucketName = "test-put-object";
            int maxNum = 3;

            // Prepare
            bool createBucketResult = await _s3Repository.CreateBucketAsync(bucketName);
            createBucketResult.ShouldBe(true);

            for (int i = 0; i < maxNum; i++)
            {
                await _s3Repository.PutObjectWithBytesAsync(
                    bucketName.ToLower(),
                    i.ToString(),
                    Convert.FromBase64String("dGVzdGNvbnRlbnQ="));
            }

            string objectKey = "2";

            // Test
            byte[] bytes = await _s3Repository.GetObjectWithBytesAsync(bucketName, objectKey);
            bytes.ShouldBe(Convert.FromBase64String("dGVzdGNvbnRlbnQ="));

            // Clean up
            await _s3Repository.PurgeBucketAsync(bucketName);
        }

        [Fact]
        public async Task TestGetObjectWithBase64Async()
        {
            string bucketName = "test-put-object";
            int maxNum = 3;

            // Prepare
            bool createBucketResult = await _s3Repository.CreateBucketAsync(bucketName);
            createBucketResult.ShouldBe(true);

            for (int i = 0; i < maxNum; i++)
            {
                await _s3Repository.PutObjectWithBase64Async(
                    bucketName.ToLower(),
                    i.ToString(),
                    "dGVzdGNvbnRlbnQ=");
            }

            string objectKey = "2";

            // Test
            string content = await _s3Repository.GetObjectWithBase64Async(bucketName, objectKey);
            content.ShouldBe("dGVzdGNvbnRlbnQ=");

            // Clean up
            await _s3Repository.PurgeBucketAsync(bucketName);
        }

        //[Fact]
        //public async Task TestPurgeBucketAsync()
        //{
        //    string bucketName = "test-list-object";
        //    await _s3Repository.PurgeBucketAsync(bucketName);
        //}

        [Fact]
        public async Task TestGetClientConfig()
        {
            await Task.Run(() =>
            {
                var config = _s3Repository.GetClientConfig();
                config.AuthenticationServiceName.ShouldBe("s3");
            });
        }
    }
}
