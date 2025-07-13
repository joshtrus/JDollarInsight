using Amazon.S3;
using Amazon.S3.Model;
using System.Text;
using System.Text.Json;

namespace JDollarInsight.Storage;

public class S3Uploader
{
    private readonly string _bucketName = "jmd-insight-data";
    private readonly IAmazonS3 _s3Client;

    public S3Uploader()
    {
        _s3Client = new AmazonS3Client(); 
    }

    public async Task UploadExchangeRateAsync(float rate)
    {
        DateTime currentDate = DateTime.Now;

        string folder = $"{currentDate:yyyyMM}";
        string fileName = $"jmd-usd_{currentDate:yyyyMMdd}.json";
        string key = $"{folder}/{fileName}";

        var json = JsonSerializer.Serialize(new
        {
            currency_pair = "JMD-USD",
            rate = rate,
            date = currentDate.ToString("dd/MM/yyy") 
        });

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = new MemoryStream(Encoding.UTF8.GetBytes(json)),
            ContentType = "application/json"
        };

        await _s3Client.PutObjectAsync(request);
        Console.WriteLine($"Uploaded to S3: {fileName}");
    }
}