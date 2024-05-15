using AuthService.Model;

namespace AuthService.Services.Interfaces;

public interface IUserService
{
    public User CreateUser(UserDto userDto);
    public User GetUserById(int userId);
    public User UpdateUser(User user, UserDto userDto);
    public User DeleteUser (int userId);
    public User GetUserByEmail(string currentUserEmail); 
    void RebuildDB();
}