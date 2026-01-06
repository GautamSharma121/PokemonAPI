using PokeAPIService.Models;

public interface IPokeApiClient
{
    Task<PokemonListResponse> GetPokemonListAsync(int limit, int offset);
    Task<PokemonDetails> GetPokemonDetailsAsync(string idOrName);
    Task<PokemonDetails> GetPokemonDetailsByUrlAsync(string url);
}
