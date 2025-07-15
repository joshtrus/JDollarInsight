using System.CommandLine;
using JDollarInsight.Storage;

namespace JDollarInsight.Commands;

public class StatsCommand
{
    public static Command Create()
    {
        var cmd = new Command("stats",
            "Summarizes the latest USD/JMD exchange rate with 7-day and 30-day averages, min/max, and standard deviation stats.");

        cmd.SetHandler(async () =>
            {
                var reader = new S3Reader();
                var prevWeekRates = await reader.GetLastDaysRates(7);
                var prevMonthRates = await reader.GetLastDaysRates(30);

                float GetAverage(List<float> rates)
                {
                    return rates.Average();
                }

                float GetMin(List<float> rates)
                {
                    return rates.Min();
                }

                float GetMax(List<float> rates)
                {
                    return rates.Max();
                }
                
                float GetStandardDeviation(List<float> rates)
                {
                   var avg = GetAverage(rates);
                   return MathF.Sqrt(rates.Select(r => (r - avg) * ( r - avg)).Average());    
                }
                
                var recentRate = prevWeekRates.OrderBy(r => r.Key).Last().Value;
                
                Console.WriteLine($" Latest USD/JMD Rate: {recentRate:F2}\n");

                var weekVals = prevWeekRates.Values.ToList();
                var monthVals = prevMonthRates.Values.ToList();

                Console.WriteLine("  7-Day Stats:");
                Console.WriteLine($"   Avg: {GetAverage(weekVals):F2}");
                Console.WriteLine($"   Min: {GetMin(weekVals):F2}");
                Console.WriteLine($"   Max: {GetMax(weekVals):F2}");
                Console.WriteLine($"   Std Dev: {GetStandardDeviation(weekVals):F2}\n");

                Console.WriteLine("  30-Day Stats:");
                Console.WriteLine($"   Avg: {GetAverage(monthVals):F2}");
                Console.WriteLine($"   Min: {GetMin(monthVals):F2}");
                Console.WriteLine($"   Max: {GetMax(monthVals):F2}");
                Console.WriteLine($"   Std Dev: {GetStandardDeviation(monthVals):F2}");
                
                var sortedRates = prevWeekRates.OrderBy(r => r.Key).ToList();

                var yesterday = sortedRates[^2];
                var today = sortedRates[^1];
                
                var percentChange = ((today.Value - yesterday.Value) / today.Value) * 100;
                Console.WriteLine($"Change from yesterday: {percentChange:+0.00;-0.00}%");
            }
        );
        return cmd;
    }
    
}