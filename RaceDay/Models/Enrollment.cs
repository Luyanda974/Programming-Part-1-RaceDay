using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Models
{//start of namespace
    public class Enrollment
    {//start of class

        [Key]
        public int EnrollmentID { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime EnrollmentDate { get; set; } = DateTime.Today;

        [Required]
        public int ParticipantID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public int CategoryID { get; set; }

    }//end of class
}//end of namespace
