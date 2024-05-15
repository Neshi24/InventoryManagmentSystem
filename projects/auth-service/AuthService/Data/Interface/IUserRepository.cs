using AuthService.Model;

namespace AuthService.Data.Interface;

public interface IUserRepository
{
    User CreateUser(User user);
    public User? GetUserById(int userId);
    public User UpdateUser(User user);
    public User? DeleteUser (int userId);
    public User GetUserByEmail(string currentUserEmail);
    void RebuildDB();
}