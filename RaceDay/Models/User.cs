using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{//start of namespace
    public class User
    {//start of class
    
        [Key]
        public int UserID {  get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName {  get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty ;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role {  get; set; } = string.Empty;
    }//end of class
}//end of namespace
