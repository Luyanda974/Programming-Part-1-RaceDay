using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{//start of namespace
    public class Event
    {//start of class
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty ;

        [Required]
        public decimal Distance { get; set; }

        [Required]
        public int OrganiserID { get; set; }

        [Required]
        public int EventTypeID { get; set; }

    }//end of class
}//end of namespace
