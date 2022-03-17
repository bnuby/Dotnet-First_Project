using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using First_Project.Data;
using First_Project.Dtos.Fight;
using First_Project.Models;
using First_Project.Services.CharacterService;
using AutoMapper;

namespace First_Project.Services.FightService
{
  public class FightService : IFightService
  {
    private readonly DataContext _context;
    private readonly ICharacterService _characterService;

    private readonly IMapper _mapper;
    public FightService(DataContext context, ICharacterService characterService, IMapper mapper)
    {
      _context = context;
      _characterService = characterService;
      _mapper = mapper;
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

        int damage = DoWeaponAttack(attacker, opponent);

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

    private static int DoWeaponAttack(Character attacker, Character opponent)
    {
      int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
      damage -= new Random().Next(opponent.Defense);
      if (damage > 0)
        opponent.HitPoints -= damage;
      return damage;
    }

    public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
    {
      var response = new ServiceResponse<AttackResultDto>();
      Console.WriteLine("write line");
      try
      {
        var attacker = await _context.Characters
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
        var opponent = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == request.OpponentId);
        if (attacker == null)
        {
          response.Success = false;
          response.Message = "Attacker not found.";
          return response;
        }
        if (opponent == null)
        {
          response.Success = false;
          response.Message = "Opponent not found.";
          return response;
        }
        var skill = attacker.Skills.FirstOrDefault(c => c.Id == request.SkillId);
        if (skill == null)
        {
          response.Success = false;
          response.Message = $"{attacker.Name} doesn't know this skill.";
          return response;
        }
        int damage = DoSkillAttack(attacker, opponent, skill);

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
      catch (Exception e)
      {
        response.Success = false;
        response.Message = e.Message;
      }

      return response;
    }

    private static int DoSkillAttack(Character attacker, Character opponent, Skill skill)
    {
      int damage = skill.Damage + (new Random().Next(attacker.intelligence));
      damage -= new Random().Next(opponent.Defense);

      if (damage > 0)
        opponent.HitPoints -= damage;
      return damage;
    }

    public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
    {
        var response = new ServiceResponse<FightResultDto>
        {
            Data = new FightResultDto()
        };

        try
        {
            var characters = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .Where(c => request.CharacterIds.Contains(c.Id))
                .ToArrayAsync();

            if (characters.Length < 2)
            {
                response.Success = false;
                response.Message = "Not enough characters.";
                return response;
            }

            bool defeated = false;
            while (!defeated)
            {
                foreach(var attacker in characters)
                {
                    var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                    var opponent = opponents[new Random().Next(opponents.Count)];

                    int damage = 0;
                    string attackUsed = string.Empty;

                    bool useWeapon = new Random().Next(2) == 0;
                    if (useWeapon)
                    {
                        attackUsed = attacker.Weapon.Name;
                        damage = DoWeaponAttack(attacker, opponent);
                    }
                    else
                    {
                        var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                        attackUsed = skill.Name;
                        damage = DoSkillAttack(attacker, opponent, skill);
                    }

                    Console.WriteLine($"damage {damage}");

                    response.Data.Log
                        .Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage.");
                    if (opponent.HitPoints <= 0)
                    {
                        defeated = true;
                        attacker.Victories++;
                        opponent.Defeats++;
                        response.Data.Log.Add($"{opponent.Name} has been defeated!");
                        response.Data.Log.Add($"{opponent.Name} win with {attacker.HitPoints} HP left!");
                        break;
                    }
                }
            }
            characters.ToList().ForEach(c => 
            {
                c.Fights++;
                c.HitPoints = 100;
            });

            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    public async Task<ServiceResponse<List<HighscoreDto>>> GetHighscore()
    {
        var characters = await _context.Characters
            .Where(c => c.Fights > 0)
            .OrderByDescending(c => c.Victories)
            .ThenBy(c => c.Defeats)
            .ToListAsync();
        var response = new ServiceResponse<List<HighscoreDto>>
        {
            Data = characters.Select(c => _mapper.Map<HighscoreDto>(c)).ToList()
        };
        return response;
    }
  }
}