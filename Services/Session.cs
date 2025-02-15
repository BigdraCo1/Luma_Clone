using Microsoft.EntityFrameworkCore;

using alma.Contexts;
using alma.Models;
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

        _context.Session.Add(session);
        _context.SaveChanges();

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

        await _context.Entry(session).Reference(s => s.User).LoadAsync();

        return session.User;
    }

    /// <summary>
    /// Gets a user from a token.
    /// </summary>
    /// <param name="token">The session token</param>
    /// <returns>The user associated with the session if it is valid, null otherwise</returns>
    public User? GetUser(string token) {
        var session = _context.Session.Find(token);
        if (session is null) {
            return null;
        }

        if (session.ExpiresAt < DateTime.Now) {
            _context.Session.Remove(session);
            _context.SaveChanges();
            return null;
        }

        session.LastUsedAt = DateTime.Now;
        _context.SaveChanges();

        _context.Entry(session).Reference(s => s.User).Load();

        return session.User;
    }

    /// <summary>
    /// Deletes a session.
    /// </summary>
    /// <param name="token">The session token</param>
    /// <returns></returns>
    public async Task DeleteSessionAsync(string token) {
        var session = await _context.Session.FindAsync(token);
        if (session is null) {
            return;
        }

        _context.Session.Remove(session);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a session.
    /// </summary>
    /// <param name="token">The session token</param>
    /// <returns></returns>
    public void DeleteSession(string token) {
        var session = _context.Session.Find(token);
        if (session is null) {
            return;
        }

        _context.Session.Remove(session);
        _context.SaveChanges();
    }

    /// <summary>
    /// Deletes all sessions for a user.
    /// </summary>
    /// <param name="user">The user to delete sessions for</param>
    /// <returns></returns>
    public async Task DeleteSessionsForUserAsync(User user) {
        var sessions = await _context.Session.Where(s => s.User == user).ToListAsync();
        _context.Session.RemoveRange(sessions);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes all sessions for a user.
    /// </summary>
    /// <param name="user">The user to delete sessions for</param>
    /// <returns></returns>
    public void DeleteSessionsForUser(User user) {
        var sessions = _context.Session.Where(s => s.User == user).ToList();
        _context.Session.RemoveRange(sessions);
        _context.SaveChanges();
    }
}
