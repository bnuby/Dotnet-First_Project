using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace First_Project.Models
{
  public class Character
  {
    public int Id { get; set; }
    public string Name { get; set; } = "Frado";
    public int HitPoints { get; set; } = 100;

    public int Strength { get; set; } = 10;

    public int Defense { get; set; } = 10;

    public int intelligence { get; set; } = 10;

    public RpgClass Class { get; set; } = RpgClass.Knight;

    public User User { get; set; }

    public Weapon Weapon { get; set; }

  }
}