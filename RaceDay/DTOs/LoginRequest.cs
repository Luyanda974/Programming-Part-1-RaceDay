using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTOs
{//start of namespace
    public class LoginRequest
    {//start of class
     //email address identifies the users account
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        //password will be checked
        [Required]
        public string Password { get; set; } = string.Empty;

    }//end of class
}//end of namespace
