using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTOs
{//start of namepsace
    public class CreateCategoryRequest
    {//start of class
        // Stores the category name, such as "5 km".
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        // Stores the category group, such as "Junior" or "Senior".
        [Required]
        [MaxLength(50)]
        public string CategoryType { get; set; } = string.Empty;

        // Identifies the event to which this category belongs.
        [Range(1, int.MaxValue)]
        public int EventID { get; set; }

    }//end of class
}//end of namespace
