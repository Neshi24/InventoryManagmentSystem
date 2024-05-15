using System.ComponentModel.DataAnnotations;
using AuthService.Data.Interface;
using AuthService.Model;
using AuthService.Services.Interfaces;
using AuthService.Services.Utility;
using AutoMapper;

namespace AuthService.Services.Implementations;
public class UserService : IUserService

{
    private readonly HashingLogic _hashingLogic;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(HashingLogic hashingLogic, IUserRepository userRepository, IMapper mapper)
    {
        _hashingLogic = hashingLogic;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public User CreateUser(UserDto userDto)
    {
        byte[] passwordHash, passwordsalt;
        _hashingLogic.GenerateHash(userDto.password, out passwordHash, out passwordsalt);
        var user = _mapper.Map<User>(userDto);
        user.HashPassword = passwordHash;
        user.SaltPassword = passwordsalt;
        return _userRepository.CreateUser(user);
    }

    public User GetUserById(int userId)
    {
        if (userId == null) throw new ValidationException("Id is invalid");
        return _userRepository.GetUserById(userId);
    }

    public User UpdateUser(User user, UserDto userDto)
    {
        //TODO Validate UserDto
        _mapper.Map(userDto, user);

        if (userDto.password != null)
        {
            byte[] passwordHash, passwordSalt;
            _hashingLogic.GenerateHash(userDto.password, out passwordHash, out passwordSalt);
            user.HashPassword = passwordHash;
            user.SaltPassword = passwordSalt;
        }

        return _userRepository.UpdateUser(user);
    }

    public User DeleteUser(int userId)
    {
        if (userId == null)
        {
            throw new ValidationException("Could not find user id");
        }

        return _userRepository.DeleteUser(userId);
    }
    
    public User GetUserByEmail(string userEmail)
    {
        
        return _userRepository.GetUserByEmail(userEmail);
    }

    public void RebuildDB()
    {
        _userRepository.RebuildDB();
    }
}