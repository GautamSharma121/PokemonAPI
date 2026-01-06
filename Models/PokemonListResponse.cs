namespace PokeAPIService.Models;

public class PokemonListResponse
{
    public List<Pokemon> Results { get; set; } = [];
}

public class Pokemon
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}