
using System.Net;
using PokeAPIService.Models;

namespace PokeAPIService.Clients;

public class PokeApiClient : IPokeApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PokeApiClient> _logger;

    public PokeApiClient(HttpClient httpClient, ILogger<PokeApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PokemonListResponse?> GetPokemonListAsync(int limit, int offset)
    {
        _logger.LogInformation("Calling PokeAPI for Pokemon list");
        return await _httpClient.GetFromJsonAsync<PokemonListResponse>(
            $"pokemon?limit={limit}&offset={offset}"
        );
    }

    public async Task<PokemonDetails?> GetPokemonDetailsByUrlAsync(string url)
    {
        _logger.LogInformation("Calling PokeAPI for Pokemon details {url}", url);
        return await _httpClient.GetFromJsonAsync<PokemonDetails>(url);
    }

    public async Task<PokemonDetails?> GetPokemonDetailsAsync(string idOrName)
    {
        _logger.LogInformation("Calling PokeAPI for Pokemon {Pokemon}", idOrName);
        var response = await _httpClient.GetAsync($"pokemon/{idOrName}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Pokemon {Pokemon} not found in PokeAPI", idOrName);
            throw new Exception($"Pokemon {idOrName} not found");
        }
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PokemonDetails>();
    }
}
