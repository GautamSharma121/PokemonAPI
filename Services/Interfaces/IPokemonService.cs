using PokeAPIService.Models;

namespace PokeAPIService.Services;

public interface IPokemonService
{
    Task<List<PokemonDto>> GetPokemonListAsync(int page, int pageSize);
    Task<PokemonSearchDto?> SearchPokemonAsync(string idOrName);
}
