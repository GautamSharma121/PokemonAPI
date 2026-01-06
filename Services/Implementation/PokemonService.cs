using Microsoft.Extensions.Caching.Memory;
using PokeAPIService.Clients;
using PokeAPIService.Models;

namespace PokeAPIService.Services;

public class PokemonService : IPokemonService
{
    private readonly IPokeApiClient _pokeClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PokemonService> _logger;

    public PokemonService(IPokeApiClient client, IMemoryCache cache, ILogger<PokemonService> logger)
    {
        _pokeClient = client;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<PokemonDto>> GetPokemonListAsync(int page, int pageSize)
    {
        _logger.LogInformation("Fetching Pokemon list. Page={Page}, PageSize={PageSize}", page, pageSize);

        var cacheKey = $"pokemon:list:page:{page}:size:{pageSize}";

        if (_cache.TryGetValue(cacheKey, out List<PokemonDto>? cached))
        {
            _logger.LogInformation("Cache hit for key {CacheKey}", cacheKey);
            return cached;
        }
        _logger.LogInformation("Cache miss for key {CacheKey}", cacheKey);
        var offset = (page - 1) * pageSize;
        var pokemonListResponse = await _pokeClient.GetPokemonListAsync(pageSize, offset);
        List<PokemonDto> pokemonListDto = [];
        if (pokemonListResponse is not null)
        {
            var detailTasks = pokemonListResponse.Results
            .Select(p => _pokeClient.GetPokemonDetailsByUrlAsync(p.Url));
            var detailsList = await Task.WhenAll(detailTasks);
            if (detailsList is not null)
            {
                pokemonListDto = detailsList.Select(detail => new PokemonDto
                {
                    Name = detail.Name,
                    Order = detail.Order,
                    Abilities = detail.Abilities
                .Select(a => a.ability.Name)
                .ToList(),
                    Type = detail.Types.First().type.Name
                }).ToList();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(cacheKey, pokemonListDto, cacheOptions);
            }
        }
        _logger.LogInformation("Fetched {Count} Pokemon from external API", pokemonListDto.Count);

        return pokemonListDto;
    }

    public async Task<PokemonSearchDto?> SearchPokemonAsync(string idOrName)
    {
        var details = await _pokeClient.GetPokemonDetailsAsync(idOrName);
        PokemonSearchDto? pokemonSearchDto = null;
        if (details is not null)
        {
            pokemonSearchDto = new PokemonSearchDto
            {
                Name = details.Name,
                Img_Url = details.Sprites.Front_default
            };
        }
        return pokemonSearchDto;
    }
}
