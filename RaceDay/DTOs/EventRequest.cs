using System;
using System.ComponentModel.DataAnnotations;


namespace RaceDay.DTOs
{//start of namepace
    public class EventRequest
    {//start of class
        // Stores the name of the event.
        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        // Stores an optional description of the event.
        [MaxLength(500)]
        public string? Description { get; set; }

        // Stores the date on which the event will take place.
        [Required]
        public DateTime EventDate { get; set; }

        // Stores the location where the event will take place.
        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        // Stores the event distance, which must be greater than zero.
        [Range(typeof(decimal), "0.01", "9999.99")]
        public decimal Distance { get; set; }

        // Identifies the event type selected for this event.
        [Range(1, int.MaxValue)]
        public int EventTypeID { get; set; }

    }//end of class
}//end of namespace
