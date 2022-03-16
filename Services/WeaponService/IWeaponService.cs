using First_Project.Dtos.Weapon;
using First_Project.Models;

namespace First_Project.Services.WeaponService
{
  public interface IWeaponService
  {

    Task<ServiceResponse<GetWeaponDto>> GetCharacterWeapon(int id);
    Task<ServiceResponse<GetWeaponDto>> AddWeapon(AddWeaponDto newWeapon);
  }
}