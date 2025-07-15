using System.CommandLine;
using JDollarInsight.Services;
using JDollarInsight.Storage;

namespace JDollarInsight.Commands;

public class DetectAnomalyCommand
{
    public static Command Create()
    {
        var cmd = new Command("detect-anomaly",
            "Detects if today’s rate is more than 2 standard deviations below the 7-day average");

        cmd.SetHandler(async () =>
            {
                var reader = new S3Reader();
                var rates = await reader.GetLastDaysRates(7);
                
                var sorted = rates.OrderBy(record => record.Key).ToList();
                var today = sorted.Last();
                var yesterday = sorted[^2];
                var past = sorted.Take(sorted.Count - 1).Select(record => record.Value).ToList();

                var avg = past.Average();
                var stdDev = MathF.Sqrt(past.Select(r => (r - avg) * (r - avg)).Average());
                var zScore = (today.Value - avg) / stdDev;
                var percentChange = ((today.Value - yesterday.Value) / yesterday.Value) * 100;

                Console.WriteLine($" Today: {today.Value:F2}, Yesterday: {yesterday.Value:F2}, Change: {percentChange:+0.00;-0.00}%");
                Console.WriteLine($"   7-day Avg: {avg:F2}, Std Dev: {stdDev:F2}, Z-score: {zScore:F2}");

                if (zScore < -2)
                {
                    Console.WriteLine(" Anomaly Detected: Today's rate is more than 2 standard deviations below the 7-day average.");
                    // TODO: Send email with rate, % change, z-score, and link to S3
                }
                else
                {
                    Console.WriteLine(" No significant anomaly detected.");
                }

            }
            
            
        );
        return cmd;
    }
}
    
    