using alma.Contexts;
using alma.Models;
using alma.Utils;

namespace alma.Services;

public interface ISessionService {
    Task<Session> GenerateAsync(User user);
    Task<User?> GetUserAsync(string token);
}

/// <summary>
/// A service for managing sessions.
/// </summary>
/// <param name="config">Injectable configuration</param>
/// <param name="context">Injectable database context</param>
public class SessionService(IConfiguration config, DatabaseContext context) : ISessionService {
    private readonly IConfiguration _config = config;
    private readonly DatabaseContext _context = context;

    /// <summary>
    /// Generates a session for a user.
    /// </summary>
    /// <param name="user">The user to generate a session for</param>
    /// <returns>The generated ession</returns>
    public async Task<Session> GenerateAsync(User user) {
        var session = new Session {
            Token = Token.Generate(_config.GetValue<int>("Session:Entropy")),
            User = user,
            ExpiresAt = DateTime.Now.AddSeconds(_config.GetValue<int>("Session:Lifetime")),
            IssuedAt = DateTime.Now
        };

        await _context.Session.AddAsync(session);
        await _context.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Gets a user from a token.
    /// </summary>
    /// <param name="token">The session token</param>
    /// <returns>The user associated with the session if it is valid, null otherwise</returns>
    public async Task<User?> GetUserAsync(string token) {
        var session = await _context.Session.FindAsync(token);
        if (session is null) {
            return null;
        }

        if (session.ExpiresAt < DateTime.Now) {
            _context.Session.Remove(session);
            await _context.SaveChangesAsync();
            return null;
        }

        session.LastUsedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return session.User;
    }
}
