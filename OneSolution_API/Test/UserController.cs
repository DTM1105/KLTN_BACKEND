using OneSolution_API.Managers;
using OneSolution_API.Models.Login;
using OneSolution_API.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;

namespace OneSolution_API.Test
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
     public class justgetid
        {
            public string id { get; set; }
            public string userId { get; set; }
        }
       [HttpPost]
        public HttpResponseMessage Register([FromBody] userModel userModel)
        {
            if (!userModel.isValid(userModel.email))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập email",
                    content = userModel
                });
            }
            if (!userModel.isValid(userModel.username) || !userModel.isValid(userModel.password))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập username/password",
                    content = userModel
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, userModel.register(userModel));

        }
        [HttpPost]
        public HttpResponseMessage VerifyRegister([FromBody] userModel userModel)
        {
            if (!userModel.isValid(userModel.maxacnhan))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập mã xác nhận",
                    content = new {
                        username = userModel.username,
                        email = userModel.email,
                        maxacnhan = userModel.maxacnhan
                    }
                });
            }
            if (!userModel.isValid(userModel.email))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập email",
                    content = new {
                        username = userModel.username,
                        email = userModel.email,
                        maxacnhan = userModel.maxacnhan
                    }
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, userModel.verifyRegister(userModel));

        }
        [HttpPost]
        public HttpResponseMessage Login([FromBody] userModel userModel)
        {
            if (!userModel.isValid(userModel.username))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập username",
                    content = new
                    {
                        username = userModel.username,
                        password = userModel.password,
                    }
                });
            }
            if (!userModel.isValid(userModel.password))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập password",
                    content = new
                    {
                        username = userModel.username,
                        password = userModel.password,
                    }
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, userModel.login(userModel));

        }
        [HttpGet]
        public HttpResponseMessage getListPatient()
        {
            userModel userModel = new userModel();
            return Request.CreateResponse(HttpStatusCode.OK, userModel.getListPatient());

        }
        [HttpGet]
        public HttpResponseMessage getListHocVi()
        {
            List <Object> list= new List<Object>() { 
                new { id=1,ten="Bác sĩ",ma="Bs" },
                new { id=2,ten="Thạc sĩ",ma="Ths" },
                new { id=3,ten="Tiến sĩ",ma="Ts" },
                new { id=4,ten="Phó giáo sư",ma="Pgs" },
                new { id=5,ten="Giáo sư",ma="Gs" },
            };

            return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
            {
                result = 1,
                message = "Success",
                content = list
            });
        }
        [HttpGet]
        public HttpResponseMessage getListRole()
        {
            List<Object> list = new List<Object>() {
                new { id=0,ten="Admin",ma="admin" },
                new { id=1,ten="Bệnh nhân",ma="patient" },
                new { id=2,ten="Bác sĩ",ma="doctor" },
                new { id=3,ten="Nhân viên phòng khám",ma="clinicAdmin" },
            };

            return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
            {
                result = 1,
                message = "Success",
                content = list
            });
        }
        [HttpPost]
        public HttpResponseMessage createUser([FromBody] userModel userModel)
        {
            if (!userModel.isValid(userModel.name))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập tên",
                    content = userModel
                });
            }
            if (!userModel.isValid(userModel.email))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập email",
                    content = userModel
                });
            }
            if (!userModel.isValid(userModel.username) || !userModel.isValid(userModel.password))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập username/password",
                    content = userModel
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, userModel.createUser(userModel));
        }
        [HttpPost]
        public HttpResponseMessage editUser([FromBody] userModel userModel)
        {
            if (!userModel.isValid(userModel.id))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập id",
                    content = userModel
                });
            }
            if (!userModel.isValid(userModel.name))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập tên",
                    content = userModel
                });
            }
            if (!userModel.isValid(userModel.email))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập email",
                    content = userModel
                });
            }
            
            return Request.CreateResponse(HttpStatusCode.OK, userModel.updateUser(userModel));

        }
        [HttpGet]
        public HttpResponseMessage getListUser(String userId)
        {
            if(String.IsNullOrEmpty(userId) || userId.Length == 0)
            {
                userId = "0";
            }
            userModel userModel = new userModel();
            return Request.CreateResponse(HttpStatusCode.OK, userModel.getListUser(userId));
        }
        [HttpPost]
        public HttpResponseMessage deleteUser(justgetid getid)
        {
            if (String.IsNullOrEmpty(getid.userId) || getid.userId.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có userId",
                    content = null
                });
            }
            userModel userModel = new userModel();
            return Request.CreateResponse(HttpStatusCode.OK, userModel.deleteUser(getid.userId));
        }
        [HttpGet]
        public HttpResponseMessage getProfile()
        {
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            String userId = "";
            if (headers.Contains("authorization"))
            {
                string token = headers.GetValues("authorization").First();

                if (Signature.CheckTokenValid(token))
                {
                    IAuthService authService = new JWTService(Utils.KeyToken);

                    List<Claim> claims = authService.GetTokenClaims(token).ToList();

                    userId = claims.FirstOrDefault(e => e.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                }
            }
            userModel userModel = new userModel();
            return Request.CreateResponse(HttpStatusCode.OK, userModel.getProfile(userId));
        }
        [HttpPost]
        public HttpResponseMessage editProfile([FromBody] userModel userModel)
        {
            if (!userModel.isValid(userModel.name))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập tên",
                    content = userModel
                });
            }
            if (!userModel.isValid(userModel.email))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập email",
                    content = userModel
                });
            }
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            String userId = "";
            if (headers.Contains("authorization"))
            {
                string token = headers.GetValues("authorization").First();

                if (Signature.CheckTokenValid(token))
                {
                    IAuthService authService = new JWTService(Utils.KeyToken);

                    List<Claim> claims = authService.GetTokenClaims(token).ToList();

                    userId = claims.FirstOrDefault(e => e.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                    {
                        result = 0,
                        message = "Token không hợp lệ vui lòng đăng nhập"
                    });
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng đăng nhập để thực hiện nghiệp vụ"
                });
            }
            userModel.id = userId;
            return Request.CreateResponse(HttpStatusCode.OK, userModel.updateProfile(userModel));

        }
    }
}