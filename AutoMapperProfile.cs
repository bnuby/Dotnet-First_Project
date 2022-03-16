using AutoMapper;
using First_Project.Dtos.Character;
using First_Project.Dtos.Weapon;
using First_Project.Models;

namespace First_Project
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
            CreateMap<Weapon, GetWeaponDto>();
        }
    }
}