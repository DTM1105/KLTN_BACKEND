using OneSolution_API.Managers;
using OneSolution_API.Models.kl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace OneSolution_API.Test
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ClinicController : ApiController
    {
       [HttpPost]
        public HttpResponseMessage createClinic([FromBody] ClinicModel clinic)
        {
            if (String.IsNullOrEmpty(clinic.maClinic) || clinic.maClinic.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có mã phòng khám",
                    content = null
                });
            }
            if (String.IsNullOrEmpty(clinic.name) || clinic.name.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có tên phòng khám",
                    content = null
                });
            }
            if (String.IsNullOrEmpty(clinic.address) || clinic.address.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có địa chỉ phòng khám",
                    content = null
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, clinic.createClinic(clinic));


        }
        [HttpPost]
        public HttpResponseMessage updateClinic([FromBody] ClinicModel clinic)
        {
            if (String.IsNullOrEmpty(clinic.id) || clinic.id.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có id phòng khám",
                    content = null
                });
            }
            if (String.IsNullOrEmpty(clinic.maClinic) || clinic.maClinic.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có mã phòng khám",
                    content = null
                });
            }
            if (String.IsNullOrEmpty(clinic.name) || clinic.name.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có tên phòng khám",
                    content = null
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, clinic.updateClinic(clinic));
        }
        [HttpGet]
        public HttpResponseMessage getListClinic()
        {
            ClinicModel clinic = new ClinicModel();
            return Request.CreateResponse(HttpStatusCode.OK, clinic.getListClinic());
        }
        [HttpGet]
        public HttpResponseMessage getDetailClinicById(String id)
        {
            if (String.IsNullOrEmpty(id) || id.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có id phòng khám",
                    content = null
                });
            }
            ClinicModel clinic = new ClinicModel();
            return Request.CreateResponse(HttpStatusCode.OK, clinic.getDetailClinic(id));
        }
        [HttpGet]
        public HttpResponseMessage deleteClinic(String id)
        {
            if (String.IsNullOrEmpty(id) || id.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có id phòng khám",
                    content = null
                });
            }
            ClinicModel clinic = new ClinicModel();
            return Request.CreateResponse(HttpStatusCode.OK, clinic.deleteClinic(id));
        }
        [HttpGet]
        public HttpResponseMessage getListClinicbyKeyword(String keyword)
        {
            if (String.IsNullOrEmpty(keyword) || keyword.Length == 0)
            {
                keyword = "0";
            }
            ClinicModel clinic = new ClinicModel();
            return Request.CreateResponse(HttpStatusCode.OK, clinic.getListClinicbyKeyword(keyword));
        }
    }
}
