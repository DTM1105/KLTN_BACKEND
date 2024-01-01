using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models.Login
{
    public class TokenModel
    {
        public String id { get; set; }
        public String username { get; set; }
        public String trangthai { get; set; }
        public String roleid { get; set; }
    }
}