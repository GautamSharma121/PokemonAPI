using System.Collections.Concurrent;

public class RefreshTokenStore : IRefreshTokenStore
{
    private static readonly ConcurrentDictionary<string, List<RefreshToken>> _store = new();

    public void Save(string userId, RefreshToken token)
    {
        var tokens = _store.GetOrAdd(userId, _ => new List<RefreshToken>());
        tokens.Add(token);
    }

    public RefreshToken? Get(string userId, string token)
    {
        return _store.TryGetValue(userId, out var tokens)
            ? tokens.FirstOrDefault(t => t.Token == token && !t.IsRevoked)
            : null;
    }

    public void Revoke(string userId, string token)
    {
        var rt = Get(userId, token);
        if (rt != null)
            rt.IsRevoked = true;
    }
}
