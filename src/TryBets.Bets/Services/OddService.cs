using System.Net.Http;
namespace TryBets.Bets.Services;

public class OddService : IOddService
{
    private readonly HttpClient _client;
    public OddService(HttpClient client)
    {
        _client = client;
    }

    public async Task<object> UpdateOdd(int MatchId, int TeamId, decimal BetValue)
    {
        string URL = $"http://localhost:5504/odd/{MatchId}/{TeamId}/{BetValue}";
        using (HttpResponseMessage response = await _client.PatchAsync(URL, null))
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return null;
    }
}