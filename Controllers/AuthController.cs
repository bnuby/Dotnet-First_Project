using First_Project.Data;
using First_Project.Dtos.User;
using First_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace First_Project.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AuthController : ControllerBase
  {

    private readonly IAuthRepository _authRepo;
    public AuthController(IAuthRepository authRepo)
    {
      _authRepo = authRepo;
    }

    [HttpPost("Login")]
    public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDto request)
    {
      var response = await _authRepo.Login(request.Username, request.Password);
      if (response.Success)
      {
        return BadRequest(response);
      }
      return Ok(response);
    }

    [HttpPost("Register")]
    public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
    {
      User user = new User
      {
        Username = request.Username
      };
      var response = await _authRepo.Register(user, request.Password);
      if (response.Success)
      {
        return BadRequest(response);
      }
      return Ok(response);
    }

  }
}