using System.Text.Json.Serialization;

namespace First_Project.Models
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum RpgClass
  {
    Knight,
    Mage,
    Cleric
  }
}