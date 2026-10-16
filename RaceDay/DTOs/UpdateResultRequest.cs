using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTOs
{//start of namespace
    public class UpdateResultRequest
    {//start of class
//stores the coreect finishing tieme
public TimeSpan FinishTime { get; set; }

        //stores correct finishing position
        [Range(1, int.MaxValue)] public int FinishPosition { get; set; }
    }//end of class
}//end of namespace
