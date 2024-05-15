using System.ComponentModel.DataAnnotations;

namespace AuthService.Model;

public class User
{   [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] HashPassword { get; set; }
    public byte[] SaltPassword { get; set; }
}
public class UserDto
{ 
    public string Username { get; set; }
    public string Email { get; set; }
    public string password { get; set; }
}