using OneSolution_API.Managers.ModelsToken;
using OneSolution_API.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace OneSolution_API.Managers
{
    public class Signature
    {
       
        public static JWTContainerModel GetJWTContainerModel(string userId, string username)
        {
            return new JWTContainerModel()
            {
                Claims = new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, username),
                    
                }
            };
        }

        public static Boolean CheckTokenValid(string token)
        {
            IAuthService authService = new JWTService(Utils.KeyToken);
            if(authService.IsTokenValid(token))
            {
                return true;
            }

            return false;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}