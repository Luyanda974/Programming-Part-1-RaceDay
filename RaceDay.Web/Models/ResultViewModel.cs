namespace RaceDay.Web.Models
{//start of nanmespace
    public class ResultViewModel
    {//end of class
     // Information returned by the Results API.
        public int ResultID { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int FinishPosition { get; set; }
        public int EnrollmentID { get; set; }

        // Additional information used to display results on the website.
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

    }//start of class
}//end of namespace
