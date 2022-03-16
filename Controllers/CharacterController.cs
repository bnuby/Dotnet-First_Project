using System.Security.Claims;
using First_Project.Dtos.Character;
using First_Project.Models;
using First_Project.Services.CharacterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace First_Project.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class CharacterController : ControllerBase
  {

    private readonly ICharacterService _characterService;

    public CharacterController(ICharacterService characterService)
    {
      _characterService = characterService;
    }


    [HttpGet]
    [Route("GetAll")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> GetAll()
    {
      return Ok(await _characterService.GetAllCharacters());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingle(int id)
    {
      return Ok(await _characterService.GetCharacterById(id));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(AddCharacterDto newCharacter)
    {
      return Ok(await _characterService.AddCharacter(newCharacter));
    }
    
    [HttpPost("Skill")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(AddCharacterSkillDto newCharacterSkill)
    {
      var result = await _characterService.AddCharacterSkill(newCharacterSkill);
      if (!result.Success)
      {
        return NotFound(result);
      }
      return Ok(result);
    }


    [HttpPut]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(UpdateCharacterDto updateCharacter)
    {
      var result = await _characterService.UpdateCharacter(updateCharacter);
      if (result.Data == null)
      {
        return NotFound(result);
      }
      return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> DeleteCharacter(int id)
    {
      var result = await _characterService.DeleteCharacter(id);
      if (result.Data == null)
      {
        return NotFound(result);
      }
      return Ok(result);
    }
  }
}