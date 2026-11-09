namespace RaceDay.Web.Models
{//start of namespace
    public class EnrollmentFormViewModel
    {//start of class
     // Stores all available events for the event dropdown.
        public List<EventViewModel> Events { get; set; } = new();

        // Stores categories belonging to the selected event.
        public List<CategoryViewModel> Categories { get; set; } = new();

        // Stores the event selected by the participant.
        public int EventID { get; set; }

        // Stores the category selected by the participant.
        public int CategoryID { get; set; }

    }//end of class
}//end of namepscae
