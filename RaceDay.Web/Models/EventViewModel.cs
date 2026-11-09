using System.Text.Json.Serialization;

namespace RaceDay.Web.Models
{//start of namespace
    public class EventViewModel
    {//start of class
     // The API returns "id", so map it to EventID.
        [JsonPropertyName("id")]
        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public decimal Distance { get; set; }

        public int OrganiserID { get; set; }

        public int EventTypeID { get; set; }

    }//end of class
}//end of namespace
