using First_Project.Dtos.Fight;
using First_Project.Models;

namespace First_Project.Services.FightService
{
    public interface IFightService
    {
         Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack);
         Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto skillAttack);
         Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequest);

         Task<ServiceResponse<List<HighscoreDto>>> GetHighscore();
    }
}