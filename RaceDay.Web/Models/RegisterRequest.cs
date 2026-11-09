namespace RaceDay.Web.Models
{//start of namespace
    public class RegisterRequest
    {//start of class
    public string FirstName {  get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // account role will be organiser/ participant
    public string Role {  get; set; } = "Participant";

    }//end of class
}//end of namespace
