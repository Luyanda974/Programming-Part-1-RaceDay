namespace RaceDay.Web.Models
{//start of namespace
    public class CategoryViewModel
    {//stat of class
        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string CategoryType { get; set; } = string.Empty;

        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
    }//end of class
}//end of namespace
