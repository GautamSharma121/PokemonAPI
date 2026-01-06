namespace PokeAPIService.Models;
public class PokemonDto
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<string> Abilities { get; set; } = [];
    public string Type { get; set; } = string.Empty;
}
