
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace UwFuncapp
{
    public static class JwtValidator
    {
        //verify the jwt token and decoded as ClaimsPrincipal
        public static ClaimsPrincipal GetPrincipal(string token, ILogger log)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var key = Encoding.UTF8.GetBytes(R.JWT_SECRET);
                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = false,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                SecurityToken securityToken;
                return tokenHandler.ValidateToken(token, validationParameters, out securityToken);
            }
            catch (Exception e)
            {
                log.LogInformation(e.ToString());
                return null;
            }
        }
    }
}