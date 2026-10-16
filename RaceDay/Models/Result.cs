using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{//start of namespace
    public class Result
    {//start of class
     [Key]
     public int ResultID { get; set; }

    [Required]
    public TimeSpan FinishTime { get; set; }

        [Required]
        public int FinishPosition { get; set; }

        [Required]
        public int EnrollmentID { get; set; }

    }//end of class
}//end of namespace
