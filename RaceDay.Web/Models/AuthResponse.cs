namespace RaceDay.Web.Models
{//start of namespce
    public class AuthResponse
    {//start of class
    public string Token { get; set; } = string.Empty;
    public int UserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

    }//end of class
}//end of namespace
