using First_Project.Dtos.Character;
using First_Project.Models;

namespace First_Project.Services.CharacterService
{
  public interface ICharacterService
  {
    Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters();

    Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id);
    Task<Character> GetRawCharacterById(int id);

    Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter);

    Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter);

    Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id);

    Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill);
  }
}