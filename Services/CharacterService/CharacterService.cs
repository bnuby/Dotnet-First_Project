using AutoMapper;
using Microsoft.EntityFrameworkCore;
using First_Project.Data;
using First_Project.Dtos.Character;
using First_Project.Models;
using System.Security.Claims;

namespace First_Project.Services.CharacterService
{

  public class CharacterService : ICharacterService
  {


    public readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
    {
      _context = context;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
    }

    private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
      var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
      var character = _mapper.Map<Character>(newCharacter);
      character.User = await _context.Users.FirstAsync(c => c.Id == GetUserId());
      _context.Characters.Add(character);
      _context.SaveChanges();
      var characters = await _context.Characters
        .Where(c => c.Id == GetUserId())
        .ToListAsync();
      serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
      serviceResponse.Message = "Create Character Successful";
      return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
      var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
      var characters = await _context.Characters
        .Include(c => c.Weapon)
        .Include(c => c.Skills)
        .Where(c => c.User.Id == GetUserId()).ToListAsync();
      serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
      serviceResponse.Message = "Get all characters";
      return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();
      var character = await _context.Characters
        .Include(c => c.Weapon)
        .Include(c => c.Skills)
        .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
      serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
      serviceResponse.Message = "Get one character";
      return serviceResponse;
    }

    public async Task<Character> GetRawCharacterById(int id)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();
      var character = await _context.Characters
        .Include(c => c.Weapon)
        .Include(c => c.Skills)
        .FirstOrDefaultAsync(c => c.Id == id);
      return character;
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();

      try
      {
        Character? character = await _context.Characters
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == updateCharacter.Id && c.User.Id == GetUserId());

        if (character != null)
        {
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
        else
        {

          serviceResponse.Success = false;
          serviceResponse.Message = "User Not found.";
        }
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
        var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
        if (character != null)
        {
          _context.Characters.Remove(character);
          _context.SaveChanges();
          serviceResponse.Data = (
              await _context.Characters
              .Where(c => c.User.Id == GetUserId())
              .ToListAsync()
            )
              .Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
        }
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Message = ex.Message;
      }
      return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
    {
      var response = new ServiceResponse<GetCharacterDto>();
      try
      {
        var character = await _context.Characters
          .Include(c => c.Weapon)
          .Include(c => c.Skills)
          .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId && c.User.Id == GetUserId());
        if (character == null)
        {
          response.Success = false;
          response.Message = "Character not found.";
          return response;
        }
        var exists = character.Skills.Any(c => c.Id == newCharacterSkill.SkillId);
        if (exists)
        {
          response.Success = false;
          response.Message = "Your character already own the skill.";
          return response;
        }


        var skill = await _context.Skills.FirstOrDefaultAsync(c => c.Id == newCharacterSkill.SkillId);
        if (skill == null)
        {
          response.Success = false;
          response.Message = "Skill not found.";
          return response;
        }

        character.Skills.Add(skill);
        await _context.SaveChangesAsync();
        response.Data = _mapper.Map<GetCharacterDto>(character);
        response.Message = "Add skill successful";
        return response;
      }
      catch (Exception e)
      {
        response.Success = false;
        response.Message = e.Message;
      }
      return response;
    }
  }
}