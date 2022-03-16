using AutoMapper;
using First_Project.Dtos.Character;

namespace First_Project.Models
{
  public class CharacterProfile : Profile
  {
    public CharacterProfile()
    {
      // Get Character Dto
      CreateMap<Character, GetCharacterDto>();

      // Create
      CreateMap<AddCharacterDto, Character>();
    }
  }
}