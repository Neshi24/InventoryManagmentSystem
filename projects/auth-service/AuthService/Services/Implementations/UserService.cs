using System.ComponentModel.DataAnnotations;
using AuthService.Data.Interface;
using AuthService.Model;
using AuthService.Services.Interfaces;
using AuthService.Services.Utility;
using AutoMapper;
using CommonPackage;
using OpenTelemetry.Trace;

namespace AuthService.Services.Implementations;
public class UserService : IUserService
{
    private readonly HashingLogic _hashingLogic;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly Tracer _tracer;

    public UserService(HashingLogic hashingLogic, IUserRepository userRepository, IMapper mapper, Tracer tracer)
    {
        _hashingLogic = hashingLogic;
        _userRepository = userRepository;
        _mapper = mapper;
        _tracer = tracer;
    }

    public User CreateUser(UserDto userDto)
    {
        using var activity = _tracer.StartActiveSpan("CreateUser service");
        try
        {
            byte[] passwordHash, passwordsalt;
            _hashingLogic.GenerateHash(userDto.password, out passwordHash, out passwordsalt);
            var user = _mapper.Map<User>(userDto);
            user.HashPassword = passwordHash;
            user.SaltPassword = passwordsalt;
            return _userRepository.CreateUser(user);
        }
        catch (Exception ex)
        {
            Monitoring.Log.Error("Unable to retrieve Create new user.", ex);
            throw;
        }
    }

    public User GetUserById(int userId)
    {
        using var activity = _tracer.StartActiveSpan("GetUserById service");
        try
        {
            if (userId == null) throw new ValidationException("Id is invalid");
            return _userRepository.GetUserById(userId);
        }
        catch (Exception ex)
        {
            Monitoring.Log.Error("Unable to retrieve Create new user.", ex);
            throw;
        }
    }

    public User UpdateUser(User user, UserDto userDto)
    {
        using var activity = _tracer.StartActiveSpan("UpdateUser service");
        try
        {
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
        catch (Exception ex)
        {
            Monitoring.Log.Error("Unable to retrieve Create new user.", ex);
            throw;
        }
    }

    public User DeleteUser(int userId)
    {
        using var activity = _tracer.StartActiveSpan("DeleteUser service");
        try
        {
            if (userId == null)
            {
                throw new ValidationException("Could not find user id");
            }

            return _userRepository.DeleteUser(userId);
        }
        catch (Exception ex)
        {
            Monitoring.Log.Error("Unable to retrieve Create new user.", ex);
            throw;
        }
    }

    public User GetUserByEmail(string userEmail)
    {
        using var activity = _tracer.StartActiveSpan("GetUserByEmail service");
        try
        {
            return _userRepository.GetUserByEmail(userEmail);
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
            _userRepository.RebuildDB();
        }
        catch (Exception ex)
        {
            Monitoring.Log.Error("Unable to retrieve Create new user.", ex);
            throw;
        }
    }
}
