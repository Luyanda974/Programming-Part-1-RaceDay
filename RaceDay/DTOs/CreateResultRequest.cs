using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTOs
{//start of namespace
    public class CreateResultRequest
    {//start of class

        //identifies the participants existing enrollment
        [Range(1, int.MaxValue)] public int EnrollmentID { get; set; }

        //stores the participamys finishing time
        public TimeSpan FinishTime { get; set; }

        //stores the participants finishing position
        [Range(1, int.MaxValue)] public int FinishPosition { get; set; }

    }//end of class
}//end of namespace
