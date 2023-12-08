using System.Text.Json;
using api.models;

namespace infrastructure.repository;

public class CurrencyApiRepository
{
    private HttpClient _httpClient;

    public CurrencyApiRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }




    public async Task<ResponseObject> GetCurrencyList()
    {
        var key = Environment.GetEnvironmentVariable("currencyapikey");
        var currencyLookupUrl = "https://api.fxapi.com/v1/latest?" +
                                 "base_currency=DKK" +
                                "&apikey=" + key;
        var currencyResponse = await _httpClient.GetAsync(currencyLookupUrl);
        return JsonSerializer.Deserialize<ResponseObject>(await currencyResponse.Content.ReadAsStringAsync()) ?? throw new InvalidOperationException();
    }
}