using System.Security.Claims;
using AutoMapper;
using First_Project.Data;
using First_Project.Dtos.Character;
using First_Project.Dtos.Weapon;
using First_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace First_Project.Services.WeaponService
{
  public class WeaponService : IWeaponService
  {

    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
      _context = context;
      _httpContextAccessor = httpContextAccessor;
      _mapper = mapper;
    }

    private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

    public async Task<ServiceResponse<GetWeaponDto>> GetCharacterWeapon(int id)
    {
      var response = new ServiceResponse<GetWeaponDto>();
      var character = await _context.Characters
          .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
      if (character == null)
      {
        response.Success = false;
        response.Message = "Character not found.";
        return response;
      }
      else
      {
        Console.WriteLine($"character {character.Id}");
        var weapon = await _context.Weapons
            .FirstOrDefaultAsync(c => c.CharacterId == character.Id);
        if (weapon == null)
        {
          response.Success = false;
          response.Message = "Weapon not found.";
          return response;
        }
        response.Data = _mapper.Map<GetWeaponDto>(weapon);
        response.Message = "Get weapon.";
      }
      return response;
    }

    public async Task<ServiceResponse<GetWeaponDto>> AddWeapon(AddWeaponDto newWeapon)
    {
      var response = new ServiceResponse<GetWeaponDto>();
      try
      {
        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId && c.User.Id == GetUserId());

        if (character == null)
        {
          response.Success = false;
          response.Message = "Character not found.";
          return response;
        }

        var weapon = new Weapon
        {
          Name = newWeapon.Name,
          Damage = newWeapon.Damage,
          Character = character
        };

        _context.Weapons.Add(weapon);
        await _context.SaveChangesAsync();

        response.Data = _mapper.Map<GetWeaponDto>(weapon);

        response.Message = "Add new weapon.";
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