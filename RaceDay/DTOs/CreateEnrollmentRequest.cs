using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTOs
{//start of namespace
    public class CreateEnrollmentRequest
    {//start of class
     // Identifies the event the participant wants to enter.
        [Range(1, int.MaxValue)]
        public int EventID { get; set; }

        // Identifies the category selected within that event.
        [Range(1, int.MaxValue)]
        public int CategoryID { get; set; }

    }//end of class
}//end of namepsace
