using System.ComponentModel.DataAnnotations;
using AuthService.Model;
using AuthService.Services.Interfaces;
using AuthService.Services.Utility;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly HashingLogic _hashingLogic;
    private readonly Authentication _authentication;
    public AuthController(IUserService userService, HashingLogic hashingLogic, Authentication authentication)
    {
        _userService = userService;
        _hashingLogic = hashingLogic;
        _authentication = authentication;
        
    }
    

    [HttpPost("Register")]
    public  ActionResult<User> CreateUser([FromBody] UserDto userDto)
    {
        try
        {
            return  _userService.CreateUser(userDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("controller went worng" +e);
        }
       
    }
    [HttpGet("{userId}")]
    public ActionResult<User> GetUserById(int userId)
    {
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
            return StatusCode(500, e.ToString());
        }
    }
    
    [HttpPut]
    [Route("update")]
    public ActionResult<User> UpdateUser(int userId, [FromBody] UserDto userDto, string password)
    {
               
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
            return StatusCode(500, e.Message);
        }
    }
    
    
    [HttpDelete]
    [Route("delete")]
    public ActionResult<User> DeleteUserById(int userId)
    {
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
            return StatusCode(500, e.ToString());
        }
    }
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserDto userDto)
    {
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