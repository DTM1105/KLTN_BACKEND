using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public Object Data { get; set; }

        public static Response Success(Object data)
        {
            var res = new Response();
            res.StatusCode = 1;
            res.Message = "Thành công";
            res.Data = data;
            return res;
        }

        public static Response Error(string msg)
        {
            var res = new Response();
            res.StatusCode = 0;
            res.Message = msg;
            res.Data = null;
            return res;
        }
    }
}