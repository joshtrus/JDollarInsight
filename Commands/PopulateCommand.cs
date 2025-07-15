using System.CommandLine;
using JDollarInsight.Storage;

namespace JDollarInsight.Commands;

public static class PopulateCommand
{
    public static Command Create()
    {
        var csvPathArg = new Argument<string>("csvPath", "Path to the CSV file containing historical exchange rate data");

        var cmd = new Command("populate", "Populate historical exchange rates from a CSV file")
        {
            csvPathArg
        };
        
        cmd.SetHandler(async (string csvPath) =>
        {
            var uploader = new S3Uploader();

            Console.WriteLine($"Reading from: {csvPath}");

            var lines = await File.ReadAllLinesAsync(csvPath);
            foreach (var line in lines.Skip(1))
            {
                var fields = line.Split(',');

                if (fields.Length < 2 || !DateTime.TryParse(fields[0].Trim('"'), out var date) || !float.TryParse(fields[1].Trim('"'), out var rate))
                {
                    Console.WriteLine($"ERROR cannot process line: {line}");
                    continue;
                }

                await uploader.UploadExchangeRateAsync(rate, date);
                Console.WriteLine($" Uploaded {rate} for {date:yyyy-MM-dd}");
            }

            Console.WriteLine("Populate complete");
        }, csvPathArg);

        return cmd;
        
    }
    
    

}