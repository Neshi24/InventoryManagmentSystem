using AuthService.Data.Interface;
using AuthService.Model;

namespace AuthService.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _authDbContext;
    
    public UserRepository(AuthDbContext authDbContext)
    {
        _authDbContext = authDbContext;
    }

    public  User CreateUser(User user)
    {
        if (_authDbContext.UsersTable.Any(u =>u.Email.Equals(user.Email)))
        {
            throw new Exception("User with this email already exists");
        }
        _authDbContext.UsersTable.Add(user);
        _authDbContext.SaveChangesAsync();
        return user;
    }

    public User? GetUserById(int userId)
    { 
        var userToFind = _authDbContext.UsersTable.FirstOrDefault(u => u.Id == (userId));
        return userToFind;
    }
    
    public User UpdateUser(User user)
    {
        _authDbContext.UsersTable.Update(user);
        _authDbContext.SaveChanges();
        return user;
    }
    
    public User? DeleteUser(int userId)
    {
        var userToDelete =  _authDbContext.UsersTable.FirstOrDefault(u => u.Id == (userId));

        if (userToDelete !=null)
        {
            _authDbContext.UsersTable.Remove(userToDelete);
            _authDbContext.SaveChangesAsync();
        }
        // returning deleted user for now. may need to change //TODO
        return userToDelete;
    }


    
    public User GetUserByEmail(string userEmail)
    {
        
        return _authDbContext.UsersTable.FirstOrDefault(u => u.Email == userEmail)!;;
    }

    
    public void RebuildDB()
    {
        _authDbContext.Database.EnsureDeleted();
        _authDbContext.Database.EnsureCreated();
    }
}