namespace RaceDay.Web.Models
{//start of namepsace
    public class ManageEnrollmentsViewModel
    {//start of class
        // Stores the events belonging to the organiser.
        public List<EventViewModel> Events { get; set; } = new();

        // Stores the enrolments for the selected event.
        public List<EnrollmentViewModel> Enrollments { get; set; } = new();

        // Stores the ID of the currently selected event.
        public int? EventID { get; set; }

    }//end of class
}//end of namepsace
