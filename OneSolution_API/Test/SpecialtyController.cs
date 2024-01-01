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
    public class SpecialtyController : ApiController
    {
       [HttpPost]
        public HttpResponseMessage createSpecialty([FromBody] SpecialtyModel spe)
        {
            if (String.IsNullOrEmpty(spe.ma) || spe.ma.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có mã chuyên khoa",
                    content = spe
                });
            }
            if (String.IsNullOrEmpty(spe.name) || spe.name.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có tên chuyên khoa",
                    content = spe
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, spe.createSpe(spe));
        }
        [HttpPost]
        public HttpResponseMessage updateSpecialty([FromBody] SpecialtyModel spe)
        {
            if (String.IsNullOrEmpty(spe.id) || spe.id.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có id phòng khám",
                    content = null
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, spe.updateSpe(spe));
        }
        [HttpGet]
        public HttpResponseMessage getListSpecialty()
        {
            SpecialtyModel spe = new SpecialtyModel();
            return Request.CreateResponse(HttpStatusCode.OK, spe.getListSpe());
        }
        [HttpGet]
        public HttpResponseMessage getDetailSpecialtyById(String id)
        {
            if (String.IsNullOrEmpty(id) || id.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có id chuyên khoa",
                    content = null
                });
            }
            SpecialtyModel spe = new SpecialtyModel();
            return Request.CreateResponse(HttpStatusCode.OK, spe.getDetailSpe(id));
        }
        [HttpGet]
        public HttpResponseMessage deleteSpecialtyById(String id)
        {
            if (String.IsNullOrEmpty(id) || id.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Không có id chuyên khoa",
                    content = null
                });
            }
            SpecialtyModel spe = new SpecialtyModel();
            return Request.CreateResponse(HttpStatusCode.OK, spe.deleteSpecialtyById(id));
        }
        [HttpGet]
        public HttpResponseMessage getListSpecialtyByKeyword(String keyword)
        {
            if (String.IsNullOrEmpty(keyword) || keyword.Length == 0)
            {
                keyword = "0";
            }
            SpecialtyModel spe = new SpecialtyModel();
            return Request.CreateResponse(HttpStatusCode.OK, spe.getListSpebyKeyword(keyword));
        }
    }
}
