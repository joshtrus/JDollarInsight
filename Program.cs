using System.CommandLine;
using JDollarInsight.Commands;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("JDollarInsight CLI");

        rootCommand.AddCommand(FetchCommand.Create());

        return await rootCommand.InvokeAsync(args);
    }
}