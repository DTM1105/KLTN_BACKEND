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
    public class DoctorController : ApiController
    {
        public class His_Medicine
        {
            public String medicineId { get; set; }
            public Double amount { get; set; }
        }
        public class changeSchedule
        {
            public String scheduleId { get; set; }
            public String fromDoctorId { get; set; }
            public String toDoctorId { get; set; }
            public String reason { get; set; }
        }
        [HttpPost]
        public HttpResponseMessage createDoctor([FromBody] DoctorModel doctor)
        {
            if (String.IsNullOrEmpty(doctor.doctorId) || doctor.doctorId.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "vui lòng nhập doctorId"
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, doctor.createDoctor(doctor));

        }
        [HttpGet]
        public HttpResponseMessage getListDoctor()
        {
            DoctorModel doctor = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, doctor.getListDoctor());

        }
        [HttpGet]
        public HttpResponseMessage getDoctorById(String doctorId)
        {
            DoctorModel doctor = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, doctor.getDoctorById(doctorId));

        }
        [HttpPost]
        public HttpResponseMessage createSchedule([FromBody] ScheduleModel sche)
        {
            if (String.IsNullOrEmpty(sche.doctorId) || sche.doctorId.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "vui lòng nhập doctorId"
                });
            }
            if (String.IsNullOrEmpty(sche.date) || sche.date.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "vui lòng chọn ngày"
                });
            }
            DoctorModel doctor = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, doctor.createSchedule(sche));

        }
        [HttpGet]
        public HttpResponseMessage getListDoctorbyToken()
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
            DoctorModel doctor = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, doctor.getListDoctorbyToken(userId));

        }
        [HttpGet]
        public HttpResponseMessage getListSchedulebyDate(String doctorId, String date)
        {
            if (String.IsNullOrEmpty(doctorId) || doctorId.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "vui lòng nhập doctorId"
                });
            }
            if (String.IsNullOrEmpty(date) || date.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "vui lòng nhập date"
                });
            }
            DoctorModel doctor = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, doctor.getListScheduleByDoctorAndDate(doctorId,date));
        }
        [HttpPost]
        public HttpResponseMessage createHistoryByDoctor([FromBody] HistoryModel history)
        {
            if ((String.IsNullOrEmpty(history.bookingCode) || history.bookingCode.Length == 0) 
                && (String.IsNullOrEmpty(history.bookingId) || history.bookingId.Length == 0)
                )
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "vui lòng nhập lịch khám"
                });
            }
            if ((String.IsNullOrEmpty(history.diagnostic) )
                )
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập chẩn đoán"
                });
            }
            if ((String.IsNullOrEmpty(history.advice))
    )
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập lời dặn"
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
            return Request.CreateResponse(HttpStatusCode.OK, history.createHistoryByDoctor(history,userId));

        }
        [HttpGet]
        public HttpResponseMessage getHistorybyId(String historyId)
        {
            if (String.IsNullOrEmpty(historyId))
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
            HistoryModel hs = new HistoryModel();
            return Request.CreateResponse(HttpStatusCode.OK, hs.getDetailHistoryById(historyId, userId));
        }
        [HttpGet]
        public HttpResponseMessage getListHistoryByDoctor(String keyword)
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
            HistoryModel hs = new HistoryModel();
            return Request.CreateResponse(HttpStatusCode.OK, hs.getListHistoryByDoctor( userId,keyword));
        }
        [HttpGet]
        public HttpResponseMessage getListSchedulebyDates(String doctorId, String fromDate, String toDate)
        {
            if (String.IsNullOrEmpty(doctorId) || doctorId.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "vui lòng nhập doctorId"
                });
            }
            if (String.IsNullOrEmpty(fromDate) || fromDate.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "vui lòng nhập date"
                });
            }
            if (String.IsNullOrEmpty(toDate) || toDate.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "vui lòng nhập date"
                });
            }
            DoctorModel doctor = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, doctor.getListSchedulebyDates(doctorId, fromDate,toDate));
        }
        [HttpGet]
        public HttpResponseMessage getListDoctorBySpecialty(String specialtyId)
        {
            DoctorModel doctor = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, doctor.getListDoctorBySpecialty(specialtyId));

        }
        [HttpGet]
        public HttpResponseMessage getListDoctorByClinic(String clinicId)
        {
            DoctorModel doctor = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, doctor.getListDoctorByClinic(clinicId));

        }
        [HttpGet]
        public HttpResponseMessage getDoanhThu(String doctorId,String fromDate, String toDate)
        {
            if (String.IsNullOrEmpty(fromDate))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng chọn từ ngày"
                });
            }
            if (String.IsNullOrEmpty(toDate))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng chọn đến ngày ngày"
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
            DoctorModel dt = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, dt.getDoanhThu(userId,doctorId,fromDate,toDate));
        }
        [HttpGet]
        public HttpResponseMessage getBookingInfo(String doctorId, String fromDate, String toDate)
        {
            if (String.IsNullOrEmpty(fromDate))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng chọn từ ngày"
                });
            }
            if (String.IsNullOrEmpty(toDate))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng chọn đến ngày ngày"
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
            DoctorModel dt = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, dt.getBookingInfo(userId, doctorId, fromDate, toDate));
        }
        [HttpGet]
        public HttpResponseMessage getDataLichHen(String doctorId, String fromDate, String toDate)
        {
            if (String.IsNullOrEmpty(fromDate))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng chọn từ ngày"
                });
            }
            if (String.IsNullOrEmpty(toDate))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng chọn đến ngày ngày"
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
            DoctorModel dt = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, dt.getDataLichHen(userId, doctorId, fromDate, toDate));
        }
        [HttpGet]
        public HttpResponseMessage getDataLoaiThuoc(String doctorId, String fromDate, String toDate)
        {
            if (String.IsNullOrEmpty(fromDate))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng chọn từ ngày"
                });
            }
            if (String.IsNullOrEmpty(toDate))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng chọn đến ngày ngày"
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
            DoctorModel dt = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, dt.getDataLoaiThuoc(userId, doctorId, fromDate, toDate));
        }
        [HttpGet]
        public HttpResponseMessage getScheduleToChange(String fromdoctorId)
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
            DoctorModel dt = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, dt.getScheduleToChange(userId, fromdoctorId));
        }
        [HttpGet]
        public HttpResponseMessage getDoctorFreeTime(String scheduleId)
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
            DoctorModel dt = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, dt.getDoctorFreeTime(userId,scheduleId));
        }
        [HttpPost]
        public HttpResponseMessage createChangeScheduleByDoctor([FromBody] changeSchedule changeSchedule)
        {
            if (String.IsNullOrEmpty(changeSchedule.fromDoctorId))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng chọn bác sĩ cần đổi lịch"
                });
            }
            if (String.IsNullOrEmpty(changeSchedule.toDoctorId))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng chọn bác sĩ thay ca"
                });
            }
            if (String.IsNullOrEmpty(changeSchedule.scheduleId))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng chọn lịch khám"
                });
            }
            if (String.IsNullOrEmpty(changeSchedule.reason))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập lý do"
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
            DoctorModel hs = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, hs.createChangeScheduleByDoctor(changeSchedule));
        }
        [HttpGet]
        public HttpResponseMessage getListChangeSchedule(String keyword)
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
            DoctorModel dt = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, dt.getListChangeSchedule(userId,keyword));
        }
        [HttpGet]
        public HttpResponseMessage approcheChangeSchedule(String changeid)
        {
            if (String.IsNullOrEmpty(changeid))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng chọn ca để duyệt"
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
            DoctorModel dt = new DoctorModel();
            return Request.CreateResponse(HttpStatusCode.OK, dt.approcheChangeSchedule(userId, changeid));
        }
    }
}