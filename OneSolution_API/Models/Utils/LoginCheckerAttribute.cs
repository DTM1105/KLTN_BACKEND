 
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneSolution_API.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace OneSolution_API.Models.Utils
{
    public class LoginCheckerAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {

        public override void OnActionExecuted(HttpActionExecutedContext actionContext)
        {
             
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            /*if (!Utils.valid_secretKey(actionContext.Request))
            {

                actionContext.Response= 
                  actionContext.Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Key không hợp lê!",
                    content = "Key không hợp lê!!"
                });
            }*/
            string userId = Utils.GetUserId_Token(actionContext.Request.Headers);
            if (!Utils.valid_user(userId))
            {
                actionContext.Response =  actionContext.Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Tài khoản không hợp lệ!",
                    content = "Tài khoản không hợp lệ!"
                });
            }


        }




    }
}