using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTOs
{//Start of samespace
    public class UpdateProfileRequest
    {//start of class

        [Required]
        [MaxLength(50)]
        //users first name is required and limited to 50 characters
        public string FirstName { get; set; } = string.Empty;

        //users surname is required and limited to 50 characters
        [Required] [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;


    }//end of namespace
}//end of class
