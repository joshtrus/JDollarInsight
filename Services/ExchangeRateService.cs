using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace JDollarInsight.Services;

public class ExchangeRateService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<float> GetExchangeRate()
    {
        //Data provided by ExchangeRate-API.com
        string url = "https://open.er-api.com/v6/latest/USD";
        
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var rate = doc.RootElement
            .GetProperty("rates")
            .GetProperty("JMD")
            .GetSingle();
        
        return rate;
    }
    
}