using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{//start of namespace
    public class CreateCategoryViewModel
    {//start of class
     // Stores the name of the category.
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        // Stores the type of participant for this category.
        [Required]
        [MaxLength(100)]
        public string CategoryType { get; set; } = string.Empty;

        // Stores the event to which this category belongs.
        [Range(1, int.MaxValue)]
        public int EventID { get; set; }
    }//end of class
}//end of namespace
