using Amazon.S3;
using Amazon.S3.Model;
using System.Text.Json;


namespace JDollarInsight.Storage;

public class ExchangeRateRecord
{
    public string currecyPair {get; set;}
    public float rate {get; set;}   
}

public class S3Reader
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName = "jmd-insight-data";

    public S3Reader()
    {
        _s3Client = new AmazonS3Client();
    }


    public async Task<Dictionary<DateTime, float>> GetLastDaysRates(int numDays)
    {
        var allRates = new Dictionary<DateTime, float>();

        var today = DateTime.Today;
        var foldersToCheck = new List<string>
        {
            $"{today:yyyyMM}/"
        };

        if (today.Day <= (numDays- 1))
            foldersToCheck.Add($"{today.AddMonths(-1):yyyyMM}/");

        foreach (var prefix in foldersToCheck)
        {
            var listRequest = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = prefix
            };

            var listResponse = await _s3Client.ListObjectsV2Async(listRequest);

            var recentFiles = listResponse.S3Objects
                .OrderByDescending(o => o.Key)
                .Take(numDays + 5);

            foreach (var file in recentFiles)
            {
                var key = Path.GetFileNameWithoutExtension(file.Key); 
                var parts = key.Split('_');
                if (parts.Length != 2 || parts[1].Length != 8)
                    continue;

                var rawDate = parts[1];
                var date = new DateTime(
                    int.Parse(rawDate.Substring(0, 4)),
                    int.Parse(rawDate.Substring(4, 2)),
                    int.Parse(rawDate.Substring(6, 2))
                );

                var getReq = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = file.Key
                };

                using var getResp = await _s3Client.GetObjectAsync(getReq);
                using var reader = new StreamReader(getResp.ResponseStream);
                var json = await reader.ReadToEndAsync();

                var parsed = JsonSerializer.Deserialize<ExchangeRateRecord>(json);
                if (parsed != null)
                    allRates[date] = parsed.rate;

                if (allRates.Count >= numDays) break;
            }

            if (allRates.Count >= numDays) break;
        }

        return allRates;
    }
}

