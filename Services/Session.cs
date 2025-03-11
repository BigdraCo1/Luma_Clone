using Microsoft.EntityFrameworkCore;

using alma.Models;
using alma.Services;
using alma.Utils;

namespace alma.Services;

public interface ISessionService {
    Task<Session> GenerateAsync(User user);
    Session Generate(User user);
    Task<User?> GetUserAsync(string token);
    User? GetUser(string token);
    Task DeleteSessionAsync(string token);
    void DeleteSession(string token);
    Task DeleteSessionsForUserAsync(User user);
    void DeleteSessionsForUser(User user);
}

/// <summary>
/// A service for managing sessions.
/// </summary>
/// <param name="config">Injectable configuration</param>
/// <param name="database">Injectable database context</param>
public class SessionService(IConfiguration config, DatabaseContext database) : ISessionService {
    private readonly IConfiguration _config = config;
    private readonly DatabaseContext _database = database;

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

        await _database.Session.AddAsync(session);
        await _database.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Generates a session for a user.
    /// </summary>
    /// <param name="user">The user to generate a session for</param>
    /// <returns>The generated ession</returns>
    public Session Generate(User user) {
        var session = new Session {
            Token = Token.Generate(_config.GetValue<int>("Session:Entropy")),
            User = user,
            ExpiresAt = DateTime.Now.AddSeconds(_config.GetValue<int>("Session:Lifetime")),
            IssuedAt = DateTime.Now
        };

        _database.Session.Add(session);
        _database.SaveChanges();

        return session;
    }

    /// <summary>
    /// Gets a user from a token.
    /// </summary>
    /// <param name="token">The session token</param>
    /// <returns>The user associated with the session if it is valid, null otherwise</returns>
    public async Task<User?> GetUserAsync(string token) {
        var session = await _database.Session.Include(s => s.User).FirstOrDefaultAsync(s => s.Token == token);

        if (session is null) {
            return null;
        }

        if (session.ExpiresAt < DateTime.Now) {
            _database.Session.Remove(session);
            await _database.SaveChangesAsync();
            return null;
        }

        session.LastUsedAt = DateTime.Now;
        await _database.SaveChangesAsync();

        return session.User;
    }

    /// <summary>
    /// Gets a user from a token.
    /// </summary>
    /// <param name="token">The session token</param>
    /// <returns>The user associated with the session if it is valid, null otherwise</returns>
    public User? GetUser(string token) {
        var session = _database.Session.Find(token);
        if (session is null) {
            return null;
        }

        if (session.ExpiresAt < DateTime.Now) {
            _database.Session.Remove(session);
            _database.SaveChanges();
            return null;
        }

        session.LastUsedAt = DateTime.Now;
        _database.SaveChanges();

        _database.Entry(session).Reference(s => s.User).Load();

        return session.User;
    }

    /// <summary>
    /// Deletes a session.
    /// </summary>
    /// <param name="token">The session token</param>
    /// <returns></returns>
    public async Task DeleteSessionAsync(string token) {
        var session = await _database.Session.FindAsync(token);
        if (session is null) {
            return;
        }

        _database.Session.Remove(session);
        await _database.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a session.
    /// </summary>
    /// <param name="token">The session token</param>
    /// <returns></returns>
    public void DeleteSession(string token) {
        var session = _database.Session.Find(token);
        if (session is null) {
            return;
        }

        _database.Session.Remove(session);
        _database.SaveChanges();
    }

    /// <summary>
    /// Deletes all sessions for a user.
    /// </summary>
    /// <param name="user">The user to delete sessions for</param>
    /// <returns></returns>
    public async Task DeleteSessionsForUserAsync(User user) {
        var sessions = await _database.Session.Where(s => s.User == user).ToListAsync();
        _database.Session.RemoveRange(sessions);
        await _database.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes all sessions for a user.
    /// </summary>
    /// <param name="user">The user to delete sessions for</param>
    /// <returns></returns>
    public void DeleteSessionsForUser(User user) {
        var sessions = _database.Session.Where(s => s.User == user).ToList();
        _database.Session.RemoveRange(sessions);
        _database.SaveChanges();
    }
}
