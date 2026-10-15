using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTOs
{//start of namespace
    public class UpdateCategory
    {//start of class
     // Defines the fields an organiser can change on a category.
        public class UpdateCategoryRequest
        {
            // Stores the updated category name.
            [Required]
            [MaxLength(100)]
            public string CategoryName { get; set; } = string.Empty;

            // Stores the updated category group.
            [Required]
            [MaxLength(50)]
            public string CategoryType { get; set; } = string.Empty;
        }

    }//emd of class
}//end of namepsace
