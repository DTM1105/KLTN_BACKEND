using OneSolution_API.Models.Utils;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace OneSolution_API.Managers.ModelsToken
{
    public class JWTContainerModel : IAuthContainerModel
    {
        public string SecretKey { get; set; } = OneSolution_API.Models.Utils. Utils.KeyToken;
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public int ExpireMinutes { get; set; } = 10080;
        public Claim[] Claims { get; set; }
    }
}