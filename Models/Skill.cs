using First_Project.Models;

namespace First_Project.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Damage { get; set; }
        public List<Character> Characters { get; set; }
    }
}