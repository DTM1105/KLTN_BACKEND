using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models.Login
{
    public class MyMember
    {
        public int ID { get; set; }
        public string LOGIN { get; set; }
        public string PASS { get; set; }
        public string NAME { get; set; }
        public string image_relogin { get; set; }
        public string TYPE { get; set; } = "0";

        public string image_url { get; set; } = "";
        public string image_folder { get; set; } = "";

        public string total_score { get; set; } = "";
        public string CONGTY_FK { get; set; }

    }
}