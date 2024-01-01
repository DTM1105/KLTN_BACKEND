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
    public class MedicineController : ApiController
    {
       [HttpPost]
        public HttpResponseMessage createMedicine([FromBody] List<MedicineModel> medicine)
        {

            if (medicine.Count <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có dữ liệu"
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
            MedicineModel md = new MedicineModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.createMedicine(medicine,userId));

        }
        [HttpGet]
        public HttpResponseMessage getListMedicine(String keyword)
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
            MedicineModel md = new MedicineModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.getListMedicine( userId,keyword));

        }
        [HttpGet]
        public HttpResponseMessage getListMedicinebyId(String medicineId)
        {
            if (String.IsNullOrEmpty(medicineId) || medicineId.Equals(""))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập medicineId"
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
            MedicineModel md = new MedicineModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.getListMedicinebyId(medicineId,userId));
        }
        [HttpGet]
        public HttpResponseMessage getListMedicinebyCodeName(String code)
        {
            if (String.IsNullOrEmpty(code) || code.Length ==0)
            {
                code = "";
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
            MedicineModel md = new MedicineModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.getListMedicinebyCodeName(code, userId));
        }
        [HttpPost]
        public HttpResponseMessage editMedicine([FromBody] MedicineModel medicine)
        {

            if (String.IsNullOrEmpty(medicine.code) || String.IsNullOrEmpty(medicine.medicineId))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập mã thuốc"
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
            MedicineModel md = new MedicineModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.editMedicine(medicine, userId));
        }
        [HttpPost]
        public HttpResponseMessage deleteMedicine([FromBody] MedicineModel medicine)
        {

            if (String.IsNullOrEmpty(medicine.medicineId))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập mã thuốc"
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
            MedicineModel md = new MedicineModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.deleteMedicine(medicine, userId));
        }
        [HttpGet]
        public HttpResponseMessage getListMedicineActive(String keyword)
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
            MedicineModel md = new MedicineModel();
            return Request.CreateResponse(HttpStatusCode.OK, md.getListMedicineActive(userId, keyword));

        }
    }
}
