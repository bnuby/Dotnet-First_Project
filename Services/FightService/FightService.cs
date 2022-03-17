using System.Diagnostics;
using First_Project.Data;
using First_Project.Dtos.Fight;
using First_Project.Models;
using First_Project.Services.CharacterService;

namespace First_Project.Services.FightService
{
  public class FightService : IFightService
  {
    private readonly DataContext _context;
    private readonly ICharacterService _characterService;
    public FightService(DataContext context, ICharacterService characterService)
    {
      _context = context;
      _characterService = characterService;
      Console.WriteLine($"characterService {characterService} characterService");
    }

    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack)
    {
      var response = new ServiceResponse<AttackResultDto>();
      try
      {
        var attacker = await _characterService
            .GetRawCharacterById(weaponAttack.AttackerId);
        var opponent = await _characterService
            .GetRawCharacterById(weaponAttack.OpponentId);

        if (attacker == null)
        {
          response.Success = false;
          response.Message = "Attacker not found.";
          return response;
        }
        else if (attacker.Weapon == null)
        {
          response.Success = false;
          response.Message = "Attacker has no weapon.";
          return response;
        }
        else if (opponent == null)
        {
          response.Success = false;
          response.Message = "Opponent not found.";
          return response;
        }

        int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
        damage -= new Random().Next(opponent.Defense);
        if (damage > 0)
          opponent.HitPoints -= damage;

        if (opponent.HitPoints <= 0)
          response.Message = $"{opponent.Name} has been defeated!";

        await _context.SaveChangesAsync();

        response.Data = new AttackResultDto
        {
          Attacker = attacker.Name,
          AttackerHP = attacker.HitPoints,
          Opponent = opponent.Name,
          OpponentHP = opponent.HitPoints,
          Damage = damage
        };
      }
      catch (Exception ex)
      {
        response.Success = false;
        response.Message = ex.Message;
      }
      return response;
    }
  }
}