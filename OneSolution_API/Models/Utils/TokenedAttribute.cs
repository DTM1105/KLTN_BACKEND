using OneSolution_API.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace OneSolution_API.Models.Utils
{
    public class TokenedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var headers = filterContext.Request.Headers;

            if (headers.Contains("Authorization"))
            {
                string token = headers.GetValues("Authorization").First();

                if (!Signature.CheckTokenValid(token))
                {
                    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new ObjectResponse
                    {
                        result = 0,
                        message = "Xác thực thông tin thất bại. Vui lòng thử lại!"
                    });
                }
            }
            else
            {
                filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new ObjectResponse
                {
                    result = 0,
                    message = "Xác thực thông tin thất bại. Vui lòng thử lại!"
                });
            }
        }
    }
}