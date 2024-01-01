using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneSolution_API.Managers
{
    public class ObjectResponse
    {
        public int result { get; set; }
        public string message { get; set; }

        public Object content { get; set; }
    }
}