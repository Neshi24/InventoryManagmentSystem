using System.Text;

namespace AuthService.Services.Utility;

public class HashingLogic
{
    public virtual void GenerateHash(string Password, out byte[] PasswordHash, out byte[] PasswordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA256())
        {
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Password));
            PasswordSalt = hmac.Key;

        }
    }
    public Boolean ValidateHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hash = new System.Security.Cryptography.HMACSHA256(passwordSalt))
        {
            var newPassHash = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < newPassHash.Length; i++)
                if (newPassHash[i] != passwordHash[i])
                    return false;
        }

        return true;
    }
}