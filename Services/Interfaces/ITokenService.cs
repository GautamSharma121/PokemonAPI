namespace PokeAPIService.Services;

public interface ITokenService
{
    public string CreateAccessToken(string userId);

    public RefreshToken CreateRefreshToken();
}
