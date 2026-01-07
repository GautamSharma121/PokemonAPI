using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokeAPIService.Services;

namespace PokemonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;
        public PokemonController(IPokemonService service)
        {
            _pokemonService = service;
        }

        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            if (pageSize > 10)
                return BadRequest("pageSize cannot be greater than 10");

            if (page < 1)
                return BadRequest("page must be greater than 0");
            var result = await _pokemonService.GetPokemonListAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue))
                return BadRequest("searchValue is required");

            var result = await _pokemonService.SearchPokemonAsync(searchValue);
            return Ok(result);
        }
    }
}
