using AuthService.Data.Interface;
using AuthService.Model;
using CommonPackage;
using OpenTelemetry.Trace;

namespace AuthService.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _authDbContext;
    private readonly Tracer _tracer;

    public UserRepository(AuthDbContext authDbContext, Tracer tracer)
    {
        _authDbContext = authDbContext;
        _tracer = tracer;
    }

    public User CreateUser(User user)
    {
        using var activity = _tracer.StartActiveSpan("CreateUser datalayer");
        try
        {
            if (_authDbContext.UsersTable.Any(u => u.Email.Equals(user.Email)))
            {
                throw new Exception("User with this email already exists");
            }
            _authDbContext.UsersTable.Add(user);
            _authDbContext.SaveChangesAsync();
            return user;
        }
        catch (Exception ex)
        {
            Monitoring.Log.Error("Unable to retrieve Create new user.", ex);
            throw;
        }
    }

    public User? GetUserById(int userId)
    {
        using var activity = _tracer.StartActiveSpan("GetUserById datalayer");
        try
        {
            var userToFind = _authDbContext.UsersTable.FirstOrDefault(u => u.Id == userId);
            return userToFind;
        }
        catch (Exception ex)
        {
            Monitoring.Log.Error("Unable to retrieve Create new user.", ex);
            throw;
        }
    }

    public User UpdateUser(User user)
    {
        using var activity = _tracer.StartActiveSpan("UpdateUser datalayer");
        try
        {
            _authDbContext.UsersTable.Update(user);
            _authDbContext.SaveChanges();
            return user;
        }
        catch (Exception ex)
        {
            Monitoring.Log.Error("Unable to retrieve Create new user.", ex);
            throw;
        }
    }

    public User? DeleteUser(int userId)
    {
        using var activity = _tracer.StartActiveSpan("DeleteUser datalayer");
        try
        {
            var userToDelete = _authDbContext.UsersTable.FirstOrDefault(u => u.Id == userId);
            if (userToDelete != null)
            {
                _authDbContext.UsersTable.Remove(userToDelete);
                _authDbContext.SaveChangesAsync();
            }
            return userToDelete;
        }
        catch (Exception ex)
        {
            Monitoring.Log.Error("Unable to retrieve Create new user.", ex);
            throw;
        }
    }

    public User GetUserByEmail(string userEmail)
    {
        using var activity = _tracer.StartActiveSpan("GetUserByEmail datalayer");
        try
        {
            return _authDbContext.UsersTable.FirstOrDefault(u => u.Email == userEmail)!;
        }
        catch (Exception ex)
        {
            Monitoring.Log.Error("Unable to retrieve Create new user.", ex);
            throw;
        }
    }

    public void RebuildDB()
    {
        try
        {
            _authDbContext.Database.EnsureDeleted();
            _authDbContext.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Monitoring.Log.Error("Unable to retrieve Create new user.", ex);
            throw;
        }
    }
}
