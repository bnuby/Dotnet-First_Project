using First_Project.Models;

namespace First_Project.Dtos.Character
{
  public class UpdateCharacterDto
  {
    public int Id { get; set; }
    public string Name { get; set; } = "Frado";
    public int HitPoints { get; set; } = 100;

    public int Strength { get; set; } = 10;

    public int Defense { get; set; } = 10;

    public int intelligence { get; set; } = 10;

    public RpgClass Class { get; set; } = RpgClass.Knight;

  }
}