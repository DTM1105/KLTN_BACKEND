using OneSolution_API.Managers;
using OneSolution_API.Models.kl;
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
    public class BookingController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage createBookingByPatient([FromBody] BookingModel booking)
        {
            if (String.IsNullOrEmpty(booking.namePatient))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập tên"
                });
            }
            if (String.IsNullOrEmpty(booking.healthstatus))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập trịêu chứng"
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
            BookingModel md = new BookingModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.createBookingByPatient(booking,userId));

        }
        [HttpPost]
        public HttpResponseMessage createBookingByAdmin([FromBody] BookingModel booking)
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
            BookingModel md = new BookingModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.createBookingByAdmin(booking,userId));

        }
        [HttpGet]
        public HttpResponseMessage getListBookingNeedToPay(String keyword)
        {
            if (String.IsNullOrEmpty(keyword))
            {
                keyword = "0";
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
            BookingModel md = new BookingModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.getListBookingNeedToPay(userId,keyword));
        }
        [HttpGet]
        public HttpResponseMessage accessToPay(String bookingId)
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
            BookingModel md = new BookingModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.accessToPay(userId, bookingId));
        }
        [HttpGet]
        public HttpResponseMessage getListHistoryByAdminClinic(String keyword)
        {
            if (String.IsNullOrEmpty(keyword))
            {
                keyword = "0";
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
            HistoryModel md = new HistoryModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.getListHistoryByAdminClinic(userId,keyword));
        }
        [HttpGet]
        public HttpResponseMessage getDetailHistoryByAdminClinic(String historyId)
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
            HistoryModel md = new HistoryModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.getDetailHistoryByAdminClinic(historyId,userId));
        }
        [HttpGet]
        public HttpResponseMessage getListBookingAccess(String macode)
        {
            if (String.IsNullOrEmpty(macode))
            {
                macode = "0";
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
            BookingModel md = new BookingModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.getListBookingAccess(userId,macode));
        }
        [HttpGet]
        public HttpResponseMessage getDetailBooking(String bookingId)
        {
            if (String.IsNullOrEmpty(bookingId))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "vui lòng nhập lịch khám"
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
            BookingModel md = new BookingModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.getDetailBookingByDoctor(bookingId,userId));
        }
        [HttpGet]
        public HttpResponseMessage accessToPayMedicine(String historyId)
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
            BookingModel md = new BookingModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.accessToPayMedicine(userId, historyId));
        }
        [HttpGet]
        public HttpResponseMessage getListBookingByPatient(String keyword)
        {
            if (String.IsNullOrEmpty(keyword))
            {
                keyword = "0";
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
            BookingModel md = new BookingModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.getListBookingByPatient(userId, keyword));
        }
        [HttpGet]
        public HttpResponseMessage getListHistotyByPatient(String keyword)
        {
            if (String.IsNullOrEmpty(keyword))
            {
                keyword = "0";
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
            BookingModel md = new BookingModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.getListHistotyByPatient(userId, keyword));
        }
    }
}
