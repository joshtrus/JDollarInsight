using System.CommandLine;
using JDollarInsight.Services;
using JDollarInsight.Storage;

namespace JDollarInsight.Commands;

public static class FetchCommand
{
    public static Command Create()
    {
        var cmd = new Command("fetch", "Fetches the current USD/JMD exchange rate and uploads to S3");

        cmd.SetHandler(async () =>
        {
            var rateService = new ExchangeRateService();
            var uploader = new S3Uploader();

            float rate = await rateService.GetExchangeRate();
            await uploader.UploadExchangeRateAsync(rate);

            Console.WriteLine($" Uploaded exchange rate: {rate}");
        });
        
        return cmd;
    }
}