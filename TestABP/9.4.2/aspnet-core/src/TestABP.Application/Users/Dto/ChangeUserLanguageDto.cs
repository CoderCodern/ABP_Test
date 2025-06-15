using System.ComponentModel.DataAnnotations;

namespace TestABP.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}