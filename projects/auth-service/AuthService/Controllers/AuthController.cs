using System.ComponentModel.DataAnnotations;
using AuthService.Model;
using AuthService.Services.Interfaces;
using AuthService.Services.Utility;
using CommonPackage;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace AuthService.Controllers;
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly HashingLogic _hashingLogic;
    private readonly Authentication _authentication;
    private readonly Tracer _tracer;
    public AuthController(IUserService userService, HashingLogic hashingLogic, Authentication authentication, Tracer tracer)
    {
        _userService = userService;
        _hashingLogic = hashingLogic;
        _authentication = authentication;
        _tracer = tracer;
    }
    

    [HttpPost("Register")]
    public  ActionResult<User> CreateUser([FromBody] UserDto userDto)
    { 
        using var activity = _tracer.StartActiveSpan("GetAllMeasurementsInMeasurementController");
        try
        {
            
            return  _userService.CreateUser(userDto);
        }
        catch (Exception e)
        {
            Monitoring.Log.Error(
                "Unable to retrieve Create new user."
            );
            Console.WriteLine(e);
            throw new Exception("controller went worng" +e);
        }
       
    }
    [HttpGet("{userId}")]
    public ActionResult<User> GetUserById(int userId)
    {
        using var activity = _tracer.StartActiveSpan("GetUserById controller");
        try
        {
            return Ok(_userService.GetUserById(userId));
            
        }
        catch (KeyNotFoundException e)
        {
            
            return NotFound("No User found at ID " + userId);
        }
        catch (Exception e)
        {
            Monitoring.Log.Error(
                "Unable to retrieve get user by id."+ userId
            );
            return StatusCode(500, e.ToString());
        }
    }
    
    [HttpPut]
    [Route("update")]
    public ActionResult<User> UpdateUser(int userId, [FromBody] UserDto userDto, string password)
    {
        using var activity = _tracer.StartActiveSpan("UpdateUser controller");
        var actualUser = _userService.GetUserById(userId);
        try
        {
            if(!_hashingLogic.ValidateHash(password, actualUser.HashPassword, actualUser.SaltPassword))
            {
                return BadRequest("Wrong password.");
            }
            else
            {
                return Ok(_userService.UpdateUser(actualUser, userDto));
            }
                    
                    
        }
        catch (KeyNotFoundException e)
        {
            return NotFound("No User found at ID " + userId);
        }
        catch (Exception e)
        {
            Monitoring.Log.Error(
                "Could not update user by that id"+ userId
            );
            return StatusCode(500, e.Message);
        }
    }
    
    
    [HttpDelete]
    [Route("delete")]
    public ActionResult<User> DeleteUserById(int userId)
    {   
        using var activity = _tracer.StartActiveSpan("DeleteUserById controller");
        try
        {
            return Ok(_userService.DeleteUser(userId));
        }
        catch (KeyNotFoundException e)
        {
            return NotFound("No specification found at ID " + userId);
        }
        catch (Exception e)
        {
            Monitoring.Log.Error(
                "Unable to delete user with that id "+ userId
            );
            return StatusCode(500, e.ToString());
        }
    }
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserDto userDto)
    {
        using var activity = _tracer.StartActiveSpan("Login controller");
        try
        {
            var currentUser = _userService.GetUserByEmail(userDto.Email.ToLower());
            if(currentUser == null)
                return BadRequest("Wrong password or Email.");
            if (currentUser.Email != userDto.Email.ToLower())
            {
                return BadRequest("Wrong password or Email.");
            }

            if (!_hashingLogic.ValidateHash(userDto.password, currentUser.HashPassword, currentUser.SaltPassword))
            {
                return BadRequest("Wrong password or Email.");
            }

            string token = _authentication.CreateToken(currentUser);
            return Ok(token);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            Monitoring.Log.Error(
                "Unable to authorise current user for login" + userDto
            );
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpPost]
    [Route("TestUsers")]
    public void AddTestUsers()
    {
        UserDto Dto = new UserDto
        {
            Username = "test",
            Email = "test",
            password = "test"
        };
        _userService.CreateUser(Dto);
    }
    
    [HttpPost]
    [Route("RebuildDB")]
    public void RebuildDB()
    {
        _userService.RebuildDB();
    }
    
}
