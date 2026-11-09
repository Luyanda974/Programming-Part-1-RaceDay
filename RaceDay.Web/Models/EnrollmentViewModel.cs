namespace RaceDay.Web.Models
{//start of namespace
    public class EnrollmentViewModel
    {//start of class
        public int EnrollmentID { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public int ParticipantID { get; set; }
        public int EventID { get; set; }
        public int CategoryID { get; set; }

        // These will display the actual names.
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }//end of class
}//end of namepsace
