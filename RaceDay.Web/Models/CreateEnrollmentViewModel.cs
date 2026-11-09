using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{//start of namepsace
    public class CreateEnrollmentViewModel
    {//start of class
        [Required]
        [Range(1, int.MaxValue)]
        public int EventID { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryID { get; set; }

    }//end of class
}//end of namepsace
