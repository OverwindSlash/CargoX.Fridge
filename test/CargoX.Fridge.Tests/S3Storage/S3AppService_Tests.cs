using CargoX.Fridge.Storage;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CargoX.Fridge.Tests.S3Storage
{
    public class S3AppService_Tests : FridgeTestBase
    {
        private readonly IS3StorageAppService _s3AppService;

        public S3AppService_Tests()
        {
            _s3AppService = Resolve<IS3StorageAppService>();
        }

        [Fact]
        public async Task TestGetObjectWithBytesAsync()
        {
            string bucketName = "test-put-object";
            int maxNum = 3;

            // Prepare
            bool createBucketResult = await _s3AppService.CreateBucketAsync(bucketName);
            createBucketResult.ShouldBe(true);

            for (int i = 0; i < maxNum; i++)
            {
                await _s3AppService.PutObjectWithBytesAsync(
                    bucketName.ToLower(),
                    i.ToString(),
                    Convert.FromBase64String("dGVzdGNvbnRlbnQ="));
            }

            string objectKey = "2";

            // Test
            byte[] bytes = await _s3AppService.GetObjectWithBytesAsync(bucketName, objectKey);
            bytes.ShouldBe(Convert.FromBase64String("dGVzdGNvbnRlbnQ="));

            // Clean up
            await _s3AppService.PurgeBucketAsync(bucketName);
        }

        //[Fact]
        //public async Task TestGetObjectWithBytesAsyncUsingMinIO()
        //{
        //    string bucketName = "2020-03-06";
        //    string objectKey = "7758A8E40D9951DA8C32376B25B01041";

        //    // Test
        //    byte[] bytes = await _s3AppService.GetObjectWithBytesAsync(bucketName, objectKey);
        //    bytes.Length.ShouldBeGreaterThan(0);
        //}
    }
}
