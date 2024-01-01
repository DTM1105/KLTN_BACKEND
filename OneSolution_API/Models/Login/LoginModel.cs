using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models.Login
{
    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string memRap { get; set; }

        public string sms_code { get; set; }
    }
}