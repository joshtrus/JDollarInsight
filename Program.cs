using System.CommandLine;
using JDollarInsight.Commands;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("JDollarInsight CLI");

        rootCommand.AddCommand(FetchCommand.Create());
        rootCommand.AddCommand(PopulateCommand.Create());
        rootCommand.AddCommand(DetectAnomalyCommand.Create());
        rootCommand.AddCommand(StatsCommand.Create());

        return await rootCommand.InvokeAsync(args);
    }
}