using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{//start of namespace
    public class Categorys
    {//start of class
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CategoryType {  get; set; } = string.Empty;

        [Required]
        public int EventID { get; set; }

    }//end of class
}//end of namepsace
