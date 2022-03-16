using First_Project.Dtos.Weapon;
using First_Project.Models;
using First_Project.Services.WeaponService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace First_Project.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class WeaponController : ControllerBase
  {
    private readonly IWeaponService _weaponService;

    public WeaponController(IWeaponService weaponService)
    {
      _weaponService = weaponService;
    }

    [HttpGet("ByCharacterId/{id}")]
    public async Task<ActionResult<ServiceResponse<List<GetWeaponDto>>>> GetCharacterWeapon(int id)
    {
      var response = await _weaponService.GetCharacterWeapon(id);
      if (response.Success == false)
      {
        return NotFound(response);
      }
      return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<GetWeaponDto>>> AddWeapon(AddWeaponDto request)
    {
      var response = await _weaponService.AddWeapon(request);
      if (response.Success == false)
      {
        return NotFound(response);
      }
      else
      {
        return Ok(response);
      }
    }
  }
}