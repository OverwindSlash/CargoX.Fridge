using System.ComponentModel.DataAnnotations;

namespace CargoX.Fridge.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}