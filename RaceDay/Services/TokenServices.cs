using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RaceDay.Services
{//start of namespace
    public class TokenServices
    {//start of class

        //stores the secret key used to sign the token
        private readonly IConfiguration _configuration;

        //recieves the applicationconfiguration 
        public TokenServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //creates a token that contains the users ID,email and role
        public string CreateToken(int userID, string email,string role)
        {
            string secretKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing.");

            //
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey) );

            //use the secret key
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //add the info about loggine in user to the token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userID.ToString()),new Claim(ClaimTypes.Email,email),new Claim(ClaimTypes.Role,role)
            };
            //build the token with the claims and signing in credentials
            var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"], audience: _configuration["Jwt:Audience"], claims:claims,expires:DateTime.UtcNow.AddHours(1),signingCredentials:credentials);

            //convert token to a stringthat can be sent to 
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }//end of class
}//end of namespace
