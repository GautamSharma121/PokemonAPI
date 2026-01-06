namespace PokeAPIService.Models;
public class PokemonDetails
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }

    public List<Ability> Abilities { get; set; } = [];
    public List<Type> Types { get; set; } = [];
    public Sprite Sprites { get; set; } = new();
}

public class Sprite
{
    public string? Front_default { get; set; }
}

public class Ability
{
    public AbilityInfo ability { get; set; } = new();
}

public class AbilityInfo
{
    public string Name { get; set; } = string.Empty;
}

public class Type
{
    public TypeInfo type { get; set; } = new();
}

public class TypeInfo
{
    public string Name { get; set; } = string.Empty;
}

