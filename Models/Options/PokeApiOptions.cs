namespace PokeAPIService.Models.Options; 
public class PokeApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; }
}
