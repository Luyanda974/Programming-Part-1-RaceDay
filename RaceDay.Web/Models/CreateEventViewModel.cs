using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{//start of namespace
    public class CreateEventViewModel
    {//start of class
     // Stores the name of the event.
        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        // Stores an optional event description.
        [MaxLength(500)]
        public string? Description { get; set; }

        // Stores the date of the event.
        [Required]
        public DateTime EventDate { get; set; }

        // Stores the event location.
        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        // Stores the event distance in kilometres.
        [Range(typeof(decimal), "0.01", "9999.99")]
        public decimal Distance { get; set; }

        // Stores the selected event type ID.
        [Range(1, int.MaxValue)]
        public int EventTypeID { get; set; }

        // Holds the event types used in the dropdown.
        public List<EventTypeViewModel> EventTypes { get; set; } = new();
    }//end of class
}//end of namespace
