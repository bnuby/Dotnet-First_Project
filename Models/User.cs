using System.ComponentModel.DataAnnotations;

namespace First_Project.Models
{
  public class User
  {
    public int Id { get; set; }
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public List<Character> Characters { get; set; }
    [Required]
    public string Role { get; set; }
  }
}