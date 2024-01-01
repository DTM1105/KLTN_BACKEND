using OneSolution_API.Managers;
using OneSolution_API.Models.Login;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Routing;
using System.Configuration;
using System.Data;
using OneSolution_API.Managers.ModelsToken;
using Newtonsoft.Json;
using System.IO;
using System.Dynamic;
using System.Globalization;
using System.Net.Http;

namespace OneSolution_API.Models.Utils
{
    public class Utils
    {

        public static String Generate_Geso_SecureHash(String orderId, String userId)
        {
            string rawData = "orderId=" + orderId + "&userId=" +userId;
            string myChecksum = sha256_hash(KeyDES + rawData);
            return myChecksum;
        }

        public static string KeyToken = "CinestarPOS_With_GESO_2019";
         public static string KeyDES = "|||geso_____onesolution|||";
       // public static string KeyDES = "Geso_And_Cinestar";
        public static string IMAGE_URL_GUEST = ConfigurationManager.AppSettings["image_url_guest"].ToString();

        internal static string getNdnIdFromMa(clsDB db, string code)
        {
            try
            {
                string query = "select pk_seq from ERP_NOIDUNGNHAP where ma ='" + code+"'";
                return db.getFirstStringValueSqlCatchException(query);
            }
            catch { }
            return "";
        }

        public static string IMAGE_PATH_GUEST = ConfigurationManager.AppSettings["image_path_guest"].ToString();
        public static string BANNER_PATH =  ConfigurationManager.AppSettings["banner_path"].ToString();
        public static string IMAGE_URL_AVATAR = ConfigurationManager.AppSettings["image_url_avatar"].ToString();
        public static string elastic_url = ConfigurationManager.AppSettings["elastic_url"].ToString();
        public static string IMAGE_URL_ROOT  = ConfigurationManager.AppSettings["img_url"].ToString(); 
        public static string ELASTIC_NODE = ConfigurationManager.AppSettings["elastic_node"].ToString();

        public static string sms_username = ConfigurationManager.AppSettings["sms_username"].ToString();
        public static string sms_password = ConfigurationManager.AppSettings["sms_password"].ToString();
        public static string sms_token = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sms_username + ":" + sms_password));
        public static string secrectKeyGesoErp ="ErpGESO2021_with_devteamthebestofworld$####!##%***^%%$@#@#";

        public static string EncryptMD5(string data)
        {
            MD5CryptoServiceProvider myMD5 = new MD5CryptoServiceProvider();
            byte[] b = System.Text.Encoding.UTF8.GetBytes(data);
            b = myMD5.ComputeHash(b);

            StringBuilder s = new StringBuilder();
            foreach (byte p in b)
            {
                s.Append(p.ToString("x").ToLower());
            }

            return s.ToString();
        }
        public static void createFolder(String path)
        {
            try
            {
                bool exists = System.IO.Directory.Exists(path);

                if (!exists)
                    System.IO.Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {

            }
        }
        public static String sha256_hash(string data)
        {
            using (var sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                // Convert byte array to a string   
                var builder = new StringBuilder();
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public static string GetQuayId_Token(HttpRequestHeaders headers)
        {
            if (headers.Contains("Authorization"))
            {
                string token = headers.GetValues("Authorization").First();

                if (Signature.CheckTokenValid(token))
                {
                    IAuthService authService = new JWTService(Utils.KeyToken);

                    List<Claim> claims = authService.GetTokenClaims(token).ToList();

                    MemRap memRap = JObject.Parse(claims.FirstOrDefault(e => e.Type.Equals(ClaimTypes.StateOrProvince)).Value).ToObject<MemRap>();
                    
                    return memRap.QUAYID.ToString();
                }
            }

            return "";
        }

        internal static HttpResponseMessage createReponse(HttpRequestMessage request, int result,string message, object content)
        {
            return request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
            {
                result = result,
                message = message,
                content = content
            }); ;
        }

        public static string GetRapId_Token(HttpRequestHeaders headers)
        {
            if (headers.Contains("Authorization"))
            {
                string token = headers.GetValues("Authorization").First();

                if (Signature.CheckTokenValid(token))
                {
                    IAuthService authService = new JWTService(Utils.KeyToken);

                    List<Claim> claims = authService.GetTokenClaims(token).ToList();

                    MemRap memRap = JObject.Parse(claims.FirstOrDefault(e => e.Type.Equals(ClaimTypes.StateOrProvince)).Value).ToObject<MemRap>();

                    return memRap.RAPID.ToString();
                }
            }

            return "";
        }

        public static string GetUserId_Token(HttpRequestHeaders headers)
        {
            try
            {
                if (headers.Contains("Authorization"))
                {
                    string token = headers.GetValues("Authorization").First();

                    if (Signature.CheckTokenValid(token))
                    {
                        IAuthService authService = new JWTService(Utils.KeyToken);

                        List<Claim> claims = authService.GetTokenClaims(token).ToList();

                        return claims.FirstOrDefault(e => e.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                    }
                }
            }catch(Exception e)
            {

            }

            return "";
        }

        public static int TimeStamp_Get()
        {
            TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1).Ticks);
            TimeSpan unixTicks = new TimeSpan(DateTime.UtcNow.Ticks) - epochTicks;
            double unixTime = unixTicks.TotalSeconds;
            return Convert.ToInt32(unixTime);
        }
        public static bool valid_user(String userId)
        {
            try
            {
                String query = " select count(pk_seq) from nhanvien where pk_seq =  " + userId;
                int sodong = clsDB.getFirstIntValueSql(clsDB.strConnSalesUp, query);
                return sodong > 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool valid_user_phoneno(String phoneNo)
        {
            try
            {
                String query = " select count(id) from guest where username =  '" + phoneNo +"'";
                int sodong = clsDB.getFirstIntValueSql(clsDB.strConnSalesUp, query);
                return sodong > 0;
            }
            catch
            {
                return false;
            }
        }

        public static string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }

        public static DateTime Get_DayOfBirth_From_Age(int age)
        {
            DateTime n = DateTime.Now;
            int year = (n.Year - age);
            DateTime bd = new DateTime(year, n.Month, n.Day);
            return bd;
        }

        public static Dictionary<String,object> parse_ROW_JSON_OBJ(DataRow row)
        {
            Dictionary<string, object> myDictionary = new Dictionary<string, object>();

            foreach (DataColumn c in row.Table.Columns)
            {
                myDictionary.Add(c.ColumnName, row[c.ColumnName]);
            }
            return myDictionary;
        }
        //public static Object parse_ROW_TO_OBJ(DataRow row) 
        //{
        //    var dic = Utils.parse_ROW_JSON_OBJ(row);
        //    return DictionaryToObject<Object>(dic);
        //}
        public static T parse_ROW_TO_OBJ<T>(DataRow row)
        {
            String strdata = XuLy.ParseDataRowToJson(row);
            var obj = JObject.Parse(strdata).ToObject<T>();
            return obj;

        }
        internal static bool valid_secretKey(HttpRequestMessage request)
        {
            if (request.Headers.Contains("secrectKeyGesoErp"))
            {
                string secretkey = request.Headers.GetValues("secrectKeyGesoErp").First();
                if (secretkey.Equals(secrectKeyGesoErp))
                {
                    return true;
                }
            }
            return false;
        }

        public static Object init_User(string id)
        {
            String query = @"SELECT PK_SEQ AS ID,DANGNHAP AS LOGIN ,TEN AS NAME ,CONGTY_FK    
                             FROM nhanvien  where pk_seq= '" + id + "'";
            DataTable dt = clsDB.getDataTableStatic(query);
            String account = XuLy.ParseDataRowToJson(dt.Rows[0]);
            if (account != null)
            {
                MyMember guest = JObject.Parse(account).ToObject<MyMember>();
                IAuthContainerModel model = Signature.GetJWTContainerModel(guest.ID.ToString(), guest.LOGIN);
                IAuthService authService = new JWTService(model.SecretKey);
                string token = authService.GenerateToken(model);
                Dictionary<string, object> myDictionary = new Dictionary<string, object>(){
                        {"userId", guest.ID.ToString()},
                        {"username", guest.LOGIN},
                        {"password", guest.PASS},
                        {"name", guest.NAME},
                        {"image_relogin", guest.image_relogin},
                        {"today", DateTime.Now} 
                    };

                string moduleId = Encryption_T.Encrypt(JsonConvert.SerializeObject(myDictionary));
                return new
                {
                    userId = guest.ID,
                    fullname = guest.NAME,
                    username = guest.LOGIN,
                    image_url = guest.image_url,
                    image_folder = guest.image_folder,
                    myToken = token,
                    refreshToken = moduleId,
                    total_score = guest.total_score,
                    type = guest.TYPE,
                    congty_fk= guest.CONGTY_FK
                };
            }
            else
            {
                return "Không lấy được thông tin user";
            }
        }



        /// <summary>
        ///  Import dữ liệu tin đăng vào Elastic Search phục vụ tìm kiếm tin đăng trên website
        /// </summary>
        /// <param name="condition">Điều kiện lọc tin đăng</param>
        /// <returns>NA</returns>
        public static void Elastic_Import(String condition)
        {
            try
            {
                String query = "\n select isnull(a.reasonnotapprove,'')reasonnotapprove,a.guest_created,a.status,a.id myId,isnull(a.search_text_1,'')search_text_1,isnull(a.search_text_2,'')search_text_2,a.token as id, a.title, dbo.ftBoDau2(a.title) title_2, a.address, a.latitude,a.longitude, a.price, convert(varchar, a.time_created,121) time_created, convert(varchar, a.time_sort,121) time_sort " +
                                        "\n	,a.Category1_fk, c1.name Category1, a.Category2_fk, c2.name Category2 " +
                                        "\n	, s1.id SubCategory1_FK , s1.name  SubCategory1 " +
                                        "\n	, s2.id SubCategory2_FK , s2.name  SubCategory2 " +
                                        "\n	, s3.id SubCategory3_FK , s3.name  SubCategory3 " +
                                        "\n	, s4.id SubCategory4_FK , s4.name  SubCategory4 " +
                                        "\n  , a.post_by, a.Purpose_FK as purpose ,  convert(varchar(max),  geography::Point(a.latitude,a.longitude, 4326) ) geolocation, isnull(a.image_url,'') image_url" +
                                        "\n  , isnull(a.number_of_image,0) number_of_image " +
                                        "\n from Article a " +
                                        "\n inner join Category c1 on c1.id = a.Category1_fk  " +
                                        "\n inner join Category c2 on c2.id = a.Category2_fk  " +
                                        "\n left join SubCategory s1 on s1.id = (select top 1 SubCategory_FK from Article_SubCategory x where x.Article_FK = a.id and x.number = 1) " +
                                        "\n left join SubCategory s2 on s2.id = (select top 1 SubCategory_FK from Article_SubCategory x where x.Article_FK = a.id and x.number = 2) " +
                                        "\n left join SubCategory s3 on s3.id = (select top 1 SubCategory_FK from Article_SubCategory x where x.Article_FK = a.id and x.number = 3) " +
                                        "\n left join SubCategory s4 on s4.id = (select top 1 SubCategory_FK from Article_SubCategory x where x.Article_FK = a.id and x.number = 4) " +
                                        "\n where 1=1 " + condition;



                DataTable dt = clsDB.getDataTableStatic(query);
                String url = Utils.elastic_url + Utils.ELASTIC_NODE + "/_doc/";
                foreach (DataRow r in dt.Rows)
                {
                    String id = r["id"].ToString();
                    String purpose = r["purpose"].ToString();

                    Dictionary<string, object> myDictionary = new Dictionary<string, object>();

                    foreach (DataColumn c in dt.Columns)
                    {
                        if (c.ColumnName.Equals("myId")) continue;
                        myDictionary.Add(c.ColumnName, r[c.ColumnName]);
                    }

                    //query = "\n select c.Page_FK,c.Page_Detail_FK, c.number,i.id,i.name,i.datatype_fk, i.inputtype_fk, isnull( i.default_value,'')default_value, isnull(i.min_value,0)min_value, isnull(i.max_value,0)max_value " +
                    //    "\n     , v.val  " +
                    //    "\n from Category_Page a" +
                    //    "\n inner join Page_Detail b on a.Page_FK = b.Page_FK " +
                    //    "\n inner join Page_Detail_Item c on b.id = c.Page_Detail_FK " +
                    //    "\n inner join Item i on i.id = c.Item_FK " +
                    //    "\n left join  Article_Item  v  on  v.ITem_FK =  c.Item_FK and v.Page_Detail_FK =  c.Page_Detail_FK and  v.Article_FK = " + r["myId"].ToString() +
                    //    "\n left join Item_Detail vd on vd.id =  v.item_detail_fk " +
                    //    "\n where a.purpose_fk = " + purpose + " and a.Category_FK =  " + r["Category2_fk"].ToString() +
                    //    "\n order by c.number";

                    query = "\n  select i.id,  isnull( v.val,'')val " +
                             "\n  from Item  i " +
                             "\n  outer apply ( select top 1 * from  Article_Item  v  where  v.ITem_FK =  i.id  and  v.Article_FK = " + r["myId"].ToString() + " ) v " +
                             "\n  where id in  " +
                             "\n    (     " +
                             "\n        select a.Item_FK	" +
                             "\n         from Category_Item a " +
                             "\n        where a.Category_FK =   " + r["Category2_fk"].ToString() + " and a.search_ui = 1  " +
                             "\n    ) " +
                             "\n  order by i.id ";

                    DataTable dtItem = clsDB.getDataTableStatic(query);
                    foreach (DataRow rItem in dtItem.Rows)
                    {
                        String key = rItem["id"].ToString();
                        Object val = rItem["val"];
                        myDictionary.Add(key, val);
                    }

                    query = " select top 1 format_date from Service_Booking_Priority where Article_FK =" + r["myId"].ToString() + " and format_date = cast ( CONVERT(smalldatetime, ROUND(CAST(getdate() AS float) * 48.0,0,1)/48.0) as datetime)  and status = 1    ";

                    long timeCountPriority = 0;
                    string timePriority = clsDB.getFirstStringValueSql(clsDB.strConnSalesUp, query);
                    if (timePriority.Length > 0)
                    {
                        DateTime d = DateTime.Parse(timePriority);
                        TimeSpan timeSpan2 = d - new DateTime(2000, 1, 1, 0, 0, 0);
                        timeCountPriority = (long)timeSpan2.TotalSeconds;
                    }
                    myDictionary.Add("myPriority", timeCountPriority);

                    query = " select top 1 format_date from Service_Booking_Boosting where Article_FK =" + r["myId"].ToString() + " and format_date = cast ( CONVERT(smalldatetime, ROUND(CAST(getdate() AS float) * 48.0,0,1)/48.0) as datetime)  and status = 1    ";

                    long timeCount = 0;
                    string time = clsDB.getFirstStringValueSql(clsDB.strConnSalesUp, query);
                    if (time.Length > 0)
                    {
                        DateTime d = DateTime.Parse(time);
                        TimeSpan timeSpan2 = d - new DateTime(2000, 1, 1, 0, 0, 0);
                        timeCount = (long)timeSpan2.TotalSeconds;
                    }
                    myDictionary.Add("boosting", timeCount);


                    query = " select distinct icon_url from Service_Booking_Sticker where isnull(icon_url,'') != '' and Article_FK =" + r["myId"].ToString() + " and format_date = cast ( CONVERT(smalldatetime, ROUND(CAST(getdate() AS float) * 48.0,0,1)/48.0) as datetime)  and status = 1    ";
                    DataTable dtS = clsDB.getDataTableStatic(query);
                    List<String> list = new List<String>();
                    foreach (DataRow rS in dtS.Rows)
                    {
                        list.Add(rS["icon_url"].ToString());
                    }
                    myDictionary.Add("sticker", list);



                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + id);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = JsonConvert.SerializeObject(myDictionary);
                        streamWriter.Write(json);
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    }
                }
            }
            catch(Exception e)
            {

            }
        }
        public static string ConvertToDecimal_Money(Decimal input)
        {
            if (input == 0)
            {
                return "0";
            }
            else
            {
                return Convert.ToDecimal(input).ToString("#,##");
            }
        }

        public static string ConvertToDecimal_Money_2(Decimal input)
        {
            string val = ConvertToDecimal_Money(input).Replace(",", "_");
            val = val.Replace(".", ",");
            val = val.Replace("_", ".");
            return val;
        }

        public static DateTime parseDatetime(string date,string format)
        {
            // "yyyy-MM-dd"
            DateTime result = DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
            return result;
        }
        public static string getDatetime()
        {
            string format = "yyyy-MM-dd";
            DateTime date1 = new DateTime();
            date1 = DateTime.Now;
            string asString = date1.ToString(format);
            return asString;
        }
        public static string FormatNummeric(decimal num)
        {
            return num.ToString("#,###,###.###");
        }
    }
}