using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{//start of namespace
    public class EventType
    {//start of class

        [Key]
        public int EventTypeID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TypeName { get; set; } = string.Empty;
        


    }//end of class
}//end of namespace
