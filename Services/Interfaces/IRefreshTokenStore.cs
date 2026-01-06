public interface IRefreshTokenStore
{
    void Save(string userId, RefreshToken token);
    RefreshToken? Get(string userId, string token);
    void Revoke(string userId, string token);
}
