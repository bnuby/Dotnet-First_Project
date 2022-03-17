using First_Project.Dtos.Fight;
using First_Project.Models;
using First_Project.Services.FightService;
using Microsoft.AspNetCore.Mvc;

namespace First_Project.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class FightController : ControllerBase
  {
    private readonly IFightService _fightService;
    public FightController(IFightService fightService)
    {
      _fightService = fightService;
    }

    [HttpPost("Weapon")]
    public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttack(WeaponAttackDto request)
    {
        var result = await _fightService.WeaponAttack(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
  }
}