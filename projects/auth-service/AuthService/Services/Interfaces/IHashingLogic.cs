namespace AuthService.Services.Interfaces;

public interface IHashingLogic
{
    void GenerateHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
}