using AutoMapper;
using Microsoft.EntityFrameworkCore;
using First_Project.Data;
using First_Project.Dtos.Character;
using First_Project.Models;

namespace First_Project.Services.CharacterService
{

  public class CharacterService : ICharacterService
  {


    public readonly IMapper _mapper;
    private readonly DataContext _context;
    public CharacterService(IMapper mapper, DataContext context)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
      var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
      var character = _mapper.Map<Character>(newCharacter);
      _context.Characters.Add(character);
      _context.SaveChanges();
      var characters = await _context.Characters.AsQueryable().ToListAsync();
      serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
      serviceResponse.Message = "Create Character Successful";
      return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
      var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
      var characters = await _context.Characters.AsQueryable().ToListAsync();
      serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
      serviceResponse.Message = "Get all characters";
      return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();
      var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
      serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
      serviceResponse.Message = "Get one character";
      return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();

      try
      {
        Character character = await _context.Characters.FirstAsync(c => c.Id == updateCharacter.Id);
        // Character character = characters.FirstOrDefault(c => c.Id == updateCharacter.Id);
        character.Name = updateCharacter.Name;
        character.HitPoints = updateCharacter.HitPoints;
        character.Strength = updateCharacter.Strength;
        character.Defense = updateCharacter.Defense;
        character.intelligence = updateCharacter.intelligence;
        character.Class = updateCharacter.Class;
        _context.SaveChanges();

        serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
        serviceResponse.Message = "Update success";
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.StackTrace);
        serviceResponse.Success = false;
        serviceResponse.Message = ex.Message;
      }
      return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
    {
      var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
      try
      {
        var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
        if (character != null)
        {
          _context.Characters.Remove(character);
          _context.SaveChanges();
        }
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Message = ex.Message;
      }
      return serviceResponse;
    }
  }
}