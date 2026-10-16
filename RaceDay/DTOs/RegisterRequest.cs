using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTOs
{//start of namespace
    public class RegisterRequest
    {//start of class

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Organiser|Participant)$", ErrorMessage = "Role must be Organiser or Participant.")]
        public string Role { get; set; } = "Participant";

    }//end of class
}//end of namespace
