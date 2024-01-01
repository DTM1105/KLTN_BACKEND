using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models.kl
{
    public class ScheduleModel
    {
        public String scheduleId { get; set; }
        public String doctorId { get; set; }
        public String clinicId { get; set; }
        public String date { get; set; }
        public String timedt { get; set; }
        public List<String> time  { get; set; }
    }
}