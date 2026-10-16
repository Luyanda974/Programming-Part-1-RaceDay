using Microsoft.AspNetCore.Identity;

namespace RaceDay.Services
{//start of namespace
    public class PasswordService
    {//start of class

        private readonly PasswordHasher<object> _passwordHasher;

        public PasswordService()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(new object(), password);
        }

        public bool VerifyPassword(string passwordHash, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(new object(), passwordHash, password);

            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }//end of class
}//end of namespace
