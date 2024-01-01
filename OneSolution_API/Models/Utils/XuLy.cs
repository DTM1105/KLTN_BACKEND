using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using System.Text.RegularExpressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

namespace OneSolution_API.Models.Utils
{

    public class XuLy
    {


        public static string img_url = ConfigurationManager.AppSettings["img_url"].ToString();
        public static string img_url_bank = ConfigurationManager.AppSettings["img_url_bank"].ToString();

        public static String getImg_url_bank(String plus_path)
        {
            return img_url_bank + plus_path;
        }

        public static String getImg_url(String plus_path)
        {
            return img_url + plus_path;
        }


        public static int KiemTraTransaction(String tableName, String columnName, String id, clsDB db)
        {
            String query = " select isnull(dangcapnhat,0) dangcapnhat from "+tableName+" where "+columnName+" = "+id;
            String dangcapnhat = db.getFirstStringValueSqlCatchException(query);
            query = "\n update " + tableName + " set dangcapnhat =  isnull(dangcapnhat,0) + 1 " +
            "\n where " + columnName + " = " + id + " and isnull(dangcapnhat,0) = " + dangcapnhat;
            return db.updateQueryReturnInt(query);
        }
        private static string MySQLEscape(string str)
        {
            return Regex.Replace(str, @"[\x00'""\b\n\r\t\cZ\\%_]",
                delegate(Match match)
                {
                    string v = match.Value;
                    switch (v)
                    {
                        case "\x00":            // ASCII NUL (0x00) character
                            return "\\0";
                        case "\b":              // BACKSPACE character
                            return "\\b";
                        case "\n":              // NEWLINE (linefeed) character
                            return "\\n";
                        case "\r":              // CARRIAGE RETURN character
                            return "\\r";
                        case "\t":              // TAB
                            return "\\t";
                        case "\u001A":          // Ctrl-Z
                            return "\\Z";
                        default:
                            return "\\" + v;
                    }
                });
        }


        public static String antiSQLInspection(String s)
        {
            if (s == null) return s;

            return MySQLEscape(s);

        }


        public XuLy()
        {
 
        }
        public static int Songaytrongthang(int month, int year)
        {
            int ngay = 0;
            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    {
                        ngay = 31;
                        break;
                    }
                case 4:
                case 6:
                case 9:
                case 11:
                    {
                        ngay = 30;
                        break;
                    }
                case 2:
                    {
                        if (year % 4 == 0)
                            ngay = 29;
                        else
                            ngay = 28;
                        break;
                    }
            }

            return ngay;
        }
        public static string ThuTrongTuan()
        {
            DayOfWeek ngay = DateTime.Now.DayOfWeek;
            string ngaythang = "";

            switch (ngay)
            {
                case DayOfWeek.Monday:
                    ngaythang = "2";
                    break;
                case DayOfWeek.Tuesday:
                    ngaythang = "3";
                    break;
                case DayOfWeek.Wednesday:
                    ngaythang = "4";
                    break;
                case DayOfWeek.Thursday:
                    ngaythang = "5";
                    break;
                case DayOfWeek.Friday:
                    ngaythang = "6";
                    break;
                case DayOfWeek.Saturday:
                    ngaythang = "7";
                    break;
                case DayOfWeek.Sunday:
                    ngaythang = "8";
                    break;
            }

            return ngaythang;
        }
        public static int LaySoNgayBC(string tungay, string denngay)
        {

            int iYear = DateTime.Now.Year;
            int iMonth = DateTime.Now.Month;
            DateTime ngay_thang_bd = DateTime.Parse(tungay);


            DateTime ngay_thang_kt = DateTime.Parse(denngay);


            return ngay_thang_kt.Subtract(ngay_thang_bd).Days;
        }
        public static bool checkDateFormat(string inputString, string formatString)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(inputString, formatString, System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static int parseInt(string value, int exceptionValue)
        {
            int so = exceptionValue;
            try { so = int.Parse(value); }
            catch { }
            return so;
        }
        public static double parseDouble(string s)
        {
            try
            {
                return double.Parse(s.Replace(",", ""));
            }
            catch (Exception e)
            {
                return 0.0;
            }
        }
        public static decimal parseStringToDecimail(string s)
        {
            try
            {
                return decimal.Parse(s.Replace(",", ""));
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static string ThuTrongTuan(String ngayht)
        {
            DayOfWeek ngay = Convert.ToDateTime(ngayht).DayOfWeek;
            string ngaythang = "";

            switch (ngay)
            {
                case DayOfWeek.Monday:
                    ngaythang = "2";
                    break;
                case DayOfWeek.Tuesday:
                    ngaythang = "3";
                    break;
                case DayOfWeek.Wednesday:
                    ngaythang = "4";
                    break;
                case DayOfWeek.Thursday:
                    ngaythang = "5";
                    break;
                case DayOfWeek.Friday:
                    ngaythang = "6";
                    break;
                case DayOfWeek.Saturday:
                    ngaythang = "7";
                    break;
                case DayOfWeek.Sunday:
                    ngaythang = "8";
                    break;
            }

            return ngaythang;
        }



        public static  int GetWeekOfMonth()
        {
            DateTime date = DateTime.Now;
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);
            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
            

        } 


        public static double getKhoangCachHaversine(double lat1, double long1, double lat2, double long2)
        {
            double R = 6371.00;
            double dLat, dLon, a, c;

            dLat = toRad(lat2 - lat1);
            dLon = toRad(long2 - long1);
            lat1 = toRad(lat1);
            lat2 = toRad(lat2);
            a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c * 1000; //m
        }

        public static double getKhoangCachGoogle(double lat1, double long1, double lat2, double long2)
        {
            CultureInfo culture = new CultureInfo("en-US");
            try
            {
                //Đo khoảng cách
                string result = "";
                string sURL;
                sURL = "http://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + lat1 + "," + long1 + "&destinations=" + lat2 + "," + long2 + "&mode=walking&sensor=false";

                WebRequest wrGETURL = WebRequest.Create(sURL);
                Stream objStream = wrGETURL.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);
                string sLine = "";
                int i = 0;
                while (sLine != null)
                {
                    i++;
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        result += sLine;
                }

                // Create an XmlReader
                using (XmlReader reader = XmlReader.Create(new StringReader(result)))
                {
                    result = "-1";
                    reader.ReadToFollowing("distance");
                    reader.ReadToFollowing("value");
                    result = reader.ReadElementContentAsString(); //Khoảng cách lấy được
                }

                return Double.Parse(result, culture);

            }
            catch (Exception ex)
            {
                return -1;

            }
        }
        

        public static double toRad(double value)
        {
            return value * Math.PI / 180;
        }


        public static int LaySoNgay(int sothang)
        {
            //int iYear = DateTime.Now.Year;
            //int iMonth = DateTime.Now.Month;

            //int tongsongay = 0;
            //for (int i = 1; i < sothang; i++)
            //{
            //    int month = iMonth - i;

            //    DateTime monthStart = new DateTime(iYear, month, 1);
            //    DateTime monthEnd = monthStart.AddMonths(1);
            //    TimeSpan ts = monthEnd.Subtract(monthStart);

            //    tongsongay += ts.Days;
            //}

            //return tongsongay;

            int iYear = DateTime.Now.Year;
            int iMonth = DateTime.Now.Month;

            DateTime ngay_thang_bd = DateTime.Now.AddMonths(((-1) * sothang) + 1);
            string thangbatdau = ngay_thang_bd.Month.ToString();
            if (thangbatdau.Length < 2)
                thangbatdau = "0" + thangbatdau;

  
            DateTime ngay_thang_kt = DateTime.Now.AddMonths(-1);
            string thangketthuc = ngay_thang_kt.Month.ToString();
            if (thangketthuc.Length < 2)
                thangketthuc = "0" + thangketthuc;

            return ngay_thang_kt.Subtract(ngay_thang_bd).Days;
        }

        public static int LaySoNgayDS(int sothang)
        {
        
            DateTime ngay_thang_ht = DateTime.Now;

            DateTime ngay_thang_bd = DateTime.Now.AddMonths( (-1) * sothang );

            int tongsongay = 0;

            tongsongay = ngay_thang_ht.Subtract(ngay_thang_bd).Days;
           
            return tongsongay;
        }

        public static string[] getCurent()
        {
            string[] thanght = new string[2];
            string thang = DateTime.Now.Month.ToString();
            if (thang.Length < 2)
                thang = "0" + thang;

            string ngay = DateTime.Now.Day.ToString();
            if (ngay.Length < 2)
                ngay = "0" + ngay;

            thanght[0] = DateTime.Now.Year.ToString() + "-" + thang + "-01";
            thanght[1] = DateTime.Now.Year.ToString() + "-" + thang + "-" + ngay;
            return thanght;

        }

        public static string[] ThangBD_KT(int sothang)
        {
            string[] thang = new string[2];

            int iYear = DateTime.Now.Year;
            int iMonth = DateTime.Now.Month;

            DateTime ngay_thang_bd = DateTime.Now.AddMonths( ( (-1) * sothang ) + 1 );
            string thangbatdau = ngay_thang_bd.Month.ToString();
            if (thangbatdau.Length < 2)
                thangbatdau = "0" + thangbatdau;

            thang[0] = ngay_thang_bd.Year.ToString() + "-" + thangbatdau + "-01";


            DateTime ngay_thang_kt = DateTime.Now.AddMonths(-1);
            string thangketthuc = ngay_thang_kt.Month.ToString();
            if (thangketthuc.Length < 2)
                thangketthuc = "0" + thangketthuc;

            //thang[0] = iYear.ToString() + "-" + thangketthuc + "-01";
            //thang[1] = iYear.ToString() + "-" + thangbatdau + "-" + LastDayOfMonth(iMonth - 1, iYear);


            thang[1] = ngay_thang_kt.Year.ToString() + "-" + thangketthuc + "-" + LastDayOfMonth(ngay_thang_kt.Month, ngay_thang_kt.Year);
            return thang;
        }

        public static string[] ThangBD_KT_TinhDS(int sothang)
        {
            string[] thang = new string[2];

            int iYear = DateTime.Now.Year;
            int iMonth = DateTime.Now.Month;

            DateTime ngay_thang_bd = DateTime.Now.AddMonths(-sothang);
            string thangbatdau = ngay_thang_bd.Month.ToString();
            if (thangbatdau.Length < 2)
                thangbatdau = "0" + thangbatdau;

            thang[0] = ngay_thang_bd.Year.ToString() + "-" + thangbatdau + "-01";


            DateTime ngay_thang_kt = DateTime.Now.AddMonths(-1);
            string thangketthuc = ngay_thang_kt.Month.ToString();
            if (thangketthuc.Length < 2)
                thangketthuc = "0" + thangketthuc;

            //thang[0] = iYear.ToString() + "-" + thangketthuc + "-01";
            //thang[1] = iYear.ToString() + "-" + thangbatdau + "-" + LastDayOfMonth(iMonth - 1, iYear);


            thang[1] = ngay_thang_kt.Year.ToString() + "-" + thangketthuc + "-" + LastDayOfMonth(ngay_thang_kt.Month, ngay_thang_kt.Year);
            return thang;
        }

        public static string LaySoNgayToiHienTai()
        {
            return DateTime.Now.Day.ToString();
        }

        public static string LastDayOfMonth(int month, int year)
        {
            string ngay = "";
            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    {
                        ngay = "31";
                        break;
                    }
                case 4:
                case 6:
                case 9:
                case 11:
                    {
                        ngay = "30";
                        break;
                    }
                case 2:
                    {
                        if (year % 4 == 0)
                            ngay = "29";
                        else
                            ngay = "28";
                        break;
                    }
            }

            return ngay;
        }

        public static string getDateTime()
        {

         

            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo serverZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, serverZone);
            return currentDateTime.ToString("yyyy-MM-dd");

        }

        public static DateTime getDate() {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo serverZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, serverZone);
            return currentDateTime;
        }

        public static string getDateTimeNamThang()
        {
			DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo serverZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, serverZone);
		
            int iYear = currentDateTime.Year;
            int iMonth = currentDateTime.Month;
            int iDay = currentDateTime.Day;

            return iYear + "-" + (iMonth < 10 ? ("0" + iMonth.ToString()) : iMonth.ToString());
        }

        public static string getDateTimeVN()
        {

            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo serverZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, serverZone);
            return currentDateTime.ToString("dd-MM-yyyy");

        }


        /**
         * Kiểm tra tọa độ có đang nằm trong đất nước Việt Nam hay không (tương đối)
         */
        public static bool dangOVietNam(string lat, string lon)
        {
            CultureInfo culture = new CultureInfo("en-US");
            double dlat = double.Parse(lat, culture);
            double dlon = double.Parse(lon, culture);
            return 8.5 < dlat && dlat < 23.4 //vĩ độ cực bắc, cực nam
                && 102.14 < dlon && dlon < 109.5; //kinh độ cực bắc, cực nam
        }

        public static bool dangOVietNam(double lat, double lon)
        {
            return 8.5 < lat && lat < 23.4 //vĩ độ cực bắc, cực nam
                && 102.14 < lon && lon < 109.5; //kinh độ cực bắc, cực nam
        }

        public static string quyen_sanpham(string userId)
        {
            return "( select sanpham_fk from nhanvien_sanpham where nhanvien_fk ='" + userId + "')";
        }


        public static string quyen_npp(string userId, int loai, string loaiId)
        {
            string today = XuLy.getDateTime();
            if (loai == 3)
            {
                //GSBH
                return " ( SELECT NPP_FK FROM NHAPP_GIAMSATBH WHERE GSBH_FK = '" + loaiId + "' AND NGAYBATDAU <= '" + today + "' AND '" + today + "' <= NGAYKETTHUC ) ";
            }
            else if (loai == 2)
            {
                //ASM
                return  " ( SELECT NPP_FK FROM NHAPP_GIAMSATBH " +
                        "   WHERE GSBH_FK IN (  " +
                        " 	    SELECT PK_SEQ FROM GIAMSATBANHANG  " +
                        " 	    WHERE KHUVUC_FK IN ( SELECT KHUVUC_FK FROM ASM_KHUVUC WHERE ASM_FK = '" + loaiId + "' ) " +
                        " 		    AND TRANGTHAI = 1 " +
                        "   ) AND NGAYBATDAU <= '" + today + "' AND '" + today + "' <= NGAYKETTHUC " +
                        " ) ";
            }
            else if (loai == 1)
            {
                //BM
                return  " ( SELECT NPP_FK FROM NHAPP_GIAMSATBH " +
                        "   WHERE GSBH_FK IN (  " +
                        " 	    SELECT PK_SEQ FROM GIAMSATBANHANG  " +
                        " 	    WHERE KHUVUC_FK IN ( SELECT PK_SEQ FROM KHUVUC WHERE VUNG_FK IN ( SELECT VUNG_FK FROM BM_CHINHANH WHERE BM_FK = '" + loaiId + "' ) ) " +
                        " 		    AND TRANGTHAI = 1 " +
                        "   ) AND NGAYBATDAU <= '" + today + "' AND '" + today + "' <= NGAYKETTHUC " +
                        " ) ";
            }

            return " ( select npp_fk from phamvihoatdong where nhanvien_fk ='" + userId + "') ";
        }

        public static string quyen_kenh(string userId)
        {
            return "( select kenh_fk as kbh_fk from nhanvien_kenh where nhanvien_fk ='" + userId + "' )";
        }

        public static string getIdNpp(string nvId, int loai, string loaiId)
        {
            return " ( SELECT PK_SEQ FROM NHAPHANPHOI " +
                    "   WHERE TRANGTHAI = 1 AND PK_SEQ IN " + quyen_npp(nvId, loai, loaiId) +
                    //"       AND PK_SEQ IN ( " +
                    //"           SELECT NPP_FK FROM NHAPP_KBH WHERE KBH_FK IN " + quyen_kenh(nvId) +
                    //"       ) " +
                    " ) ";
        }

        public static string getIdDdkd(string nvId, int loai, string loaiId)
        {
            return " ( SELECT PK_SEQ FROM DAIDIENKINHDOANH WHERE TRANGTHAI = 1 AND NPP_FK IN " + getIdNpp(nvId, loai, loaiId) + " ) ";
        }

        public static string getIdGsbh(string nvId, int loai, string loaiId)
        {
            return " ( SELECT PK_SEQ FROM GIAMSATBANHANG WHERE TRANGTHAI = 1 AND PK_SEQ IN ( SELECT GSBH_FK FROM GSBH_KHUVUC WHERE KHUVUC_FK IN ( SELECT KHUVUC_FK FROM NHAPHANPHOI WHERE PK_SEQ IN " + getIdNpp(nvId, loai, loaiId) + " ) ) ) ";
        }

        public static string getIdAsm(string nvId, int loai, string loaiId)
        {
            return " ( SELECT PK_SEQ FROM ASM WHERE TRANGTHAI = 1 AND PK_SEQ IN ( SELECT ASM_FK FROM ASM_KHUVUC WHERE KHUVUC_FK IN ( SELECT KHUVUC_FK FROM NHAPHANPHOI WHERE PK_SEQ IN " + getIdNpp(nvId, loai, loaiId) + " ) ) ) ";
        }

        public static string getIdBm(string nvId, int loai, string loaiId)
        {
            return " ( SELECT PK_SEQ FROM BM WHERE TRANGTHAI = 1 AND PK_SEQ IN ( SELECT BM_FK FROM BM_CHINHANH WHERE VUNG_FK IN ( SELECT VUNG_FK FROM KHUVUC WHERE TRANGTHAI = 1 AND PK_SEQ IN ( SELECT KHUVUC_FK FROM NHAPHANPHOI WHERE PK_SEQ IN " + getIdNpp(nvId, loai, loaiId) + " ) ) ) ) ";
        }


        public static string ParseDataRowToJson(DataRow datarow)
        {
            var dict = new Dictionary<string, object>();
            foreach (DataColumn col in datarow.Table.Columns)
            {
                dict.Add(col.ColumnName, datarow[col]);
            }

            var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            return jsSerializer.Serialize(dict);
        }
        public static string ParseDataTableToJSon(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = 2147483644;

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }

            dt.Clear();
            dt.Clone();
            return serializer.Serialize(rows);

        }

        public static IEnumerable<Dictionary<string, object>> Serialize(SqlDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            reader.Close();
            return results;
        }
        public static IEnumerable<Dictionary<string, object>> Serialize(MySqlDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            return results;
        }

        private static Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }

        private static Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                              MySqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }

        public static int parseInt(string s)
        {
            try
            {
                return int.Parse(s.Replace(",", ""));
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static string BoDau(string ip_str_change)
        {
            ip_str_change = ip_str_change.Trim();
            Regex v_reg_regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string v_str_FormD = ip_str_change.Normalize(NormalizationForm.FormD);
            string kq = v_reg_regex.Replace(v_str_FormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            return kq.Trim();
            //return Regex.Replace(kq.Trim(), " ", "-");
        }
       
        public static bool tuanChan()
        {
            DateTime date = DateTime.Now;
            var cultureInfo = CultureInfo.CurrentCulture;
            var calendar = cultureInfo.Calendar;

            var calendarWeekRule = cultureInfo.DateTimeFormat.CalendarWeekRule;
            var firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            var lastDayOfWeek = cultureInfo.LCID == 1033 //En-us
                                ? DayOfWeek.Saturday
                                : DayOfWeek.Sunday;

            var lastDayOfYear = new DateTime(date.Year, 12, 31);

            var weekNumber = calendar.GetWeekOfYear(date, calendarWeekRule, firstDayOfWeek);

            //Check if this is the last week in the year and it doesn`t occupy the whole week
            int rs = weekNumber == 53 && lastDayOfYear.DayOfWeek != lastDayOfWeek
                   ? 1
                   : weekNumber;
            //int week = GetWeekOfMonth(DateTime.Today);

            return rs % 2 == 0;
        }

        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }


        public static string UploadToAzure(MemoryStream ms, string folderName, string fileName)
        {
            try
            {



                string date = XuLy.getDateTime(); 

                FileStream fs = new FileStream(fileName, FileMode.Create, System.IO.FileAccess.Write);
                 

                ms.WriteTo(fs);
                ms.Close();
                fs.Close();
                ms.Dispose();
                fs.Dispose();

                return fileName;

                return fileName;
            }
            catch (Exception ex)
            {
                return "Error Upload: " + ex.Message;
            }
        }

        public static void SaveTxt(string path, string data)
        {
            StreamWriter sw = new StreamWriter(path, true);
            try
            {
                sw.WriteLine(DateTime.Now.ToString() + "." + data );
            }
            catch
            {
               
            }

            sw.Close();
        }

       public static  DataTable Trungbay_ChupanhTb(string khId, String hinhAnhBytes1, string ddkdId, string cttbId)
       {
            string ngay = DateTime.Now.Year + "-" + (DateTime.Now.Month > 9 ? DateTime.Now.Month + "" : "0" + DateTime.Now.Month) + "-" + (DateTime.Now.Day > 9 ? DateTime.Now.Day + "" : "0" + DateTime.Now.Day);
            DataTable dtResult = new DataTable("ChupAnh");
            dtResult.Columns.Add(new DataColumn("RESULT", Type.GetType("System.String")));
            dtResult.Columns.Add(new DataColumn("MSG", Type.GetType("System.String")));

            DataRow dong = dtResult.NewRow();
            dong["RESULT"] = "1";
            dong["MSG"] = "Lưu thành công!";
            dtResult.Rows.Add(dong);

            if (khId == null || khId.Length <= 0)
            {
                dong["RESULT"] = "0";
                dong["MSG"] = "Dữ liệu truyền (id khách hàng) không đúng định dạng!";
                return dtResult;
            }

            clsDB db = null;
            try
            {
                db = new clsDB();
                string date = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
               
                string filePath = ConfigurationManager.AppSettings["FilePath"];
                string fileName = "chupanhtb_" + khId + "_" + cttbId + "_" + date + ".jpg";
                string fullPath = filePath + "cttrungbay\\" + fileName;
                MemoryStream ms = new MemoryStream(Convert.FromBase64String(hinhAnhBytes1.Replace("\r\n", "")));
                UploadToAzure(ms, "cttrungbay", fileName);
                ms.Close();
                ms.Dispose();

                string query = "update DKTRUNGBAY_KHACHHANG set ImageFilePath2 = '" + fileName + "', THOIDIEM2 = dbo.GetLocalDate(DEFAULT), DDKD_FK = " + ddkdId + " where DKTRUNGBAY_FK = (SELECT PK_SEQ FROM DANGKYTRUNGBAY WHERE CTTRUNGBAY_FK = '" + cttbId + "') and KHACHHANG_FK = '" + khId + "' and (DDKD_FK IS NULL OR DDKD_FK = " + ddkdId + ")";


                if (db.updateQueryReturnInt(query) != 1)
                {
                    db.CLose_Connection();
                    dong["RESULT"] = "0";
                    dong["MSG"] = "Xảy ra lỗi khi đăng ký chương trình trưng bày(2)";
                    return dtResult;
                }

            }
            catch (Exception ex)
            {
                db.CLose_Connection();
                dong["RESULT"] = "0";
                dong["MSG"] = "Xảy ra lỗi khi đăng ký" + khId + " (" + ex.Message + ")";
            }
            finally
            {
                db.CLose_Connection();
            }

            return dtResult;
       }
        public static DataTable Trungbay_DangkyTb(string khId, String hinhAnhBytes1, string ddkdId, string cttbId, int soxuat, string dai, string rong, string cao)
        {
            string ngay = DateTime.Now.Year + "-" + (DateTime.Now.Month > 9 ? DateTime.Now.Month + "" : "0" + DateTime.Now.Month) + "-" + (DateTime.Now.Day > 9 ? DateTime.Now.Day + "" : "0" + DateTime.Now.Day);
            DataTable dtResult = new DataTable("ChupAnh");
            dtResult.Columns.Add(new DataColumn("RESULT", Type.GetType("System.String")));
            dtResult.Columns.Add(new DataColumn("MSG", Type.GetType("System.String")));

            DataRow dong = dtResult.NewRow();
            dong["RESULT"] = "1";
            dong["MSG"] = "Lưu thành công!";
            dtResult.Rows.Add(dong);

            if (khId == null || khId.Length <= 0)
            {
                dong["RESULT"] = "0";
                dong["MSG"] = "1.Dữ liệu truyền (id khách hàng) không đúng định dạng!";
                return dtResult;
            }

            clsDB db = null;

            try
            {
                db = new clsDB();

                string query = "select 1 from DANGKYTRUNGBAY where CTTRUNGBAY_FK = '" + cttbId + "'  and TRANGTHAI = 0";

                string datao = db.getFirstStringValueSqlCatchException(query);

                if (datao == null || !datao.ToString().Equals("1"))
                {
                    query = "insert into DANGKYTRUNGBAY(CTTRUNGBAY_FK, NGAYDANGKY, NGAYSUA, NGUOITAO, NGUOISUA, TRANGTHAI) " +
                            "values(" + cttbId + ", '" + ngay + "', '" + ngay + "', 100002, 100002, 0)";
                    if (db.updateQueryReturnInt(query) <= 0)
                    {
                        db.CLose_Connection();
                        dong["RESULT"] = "0";
                        dong["MSG"] = "2.Xảy ra lỗi khi đăng ký trưng bày";
                        return dtResult;
                    }
                    else
                        datao = "1";
                }
                if (datao.ToString().Equals("1"))
                {
                    string date = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    


                    string filePath = ConfigurationManager.AppSettings["FilePath"];
                    string fileName = "dangkytb_" + khId + "_" + cttbId + "_" + date + ".jpg";
                    string fullPath = filePath + "cttrungbay\\" + fileName;
                    MemoryStream ms = new MemoryStream(Convert.FromBase64String(hinhAnhBytes1.Replace("\r\n", "")));
                    UploadToAzure(ms, "cttrungbay", fileName);
                    ms.Close();
                    ms.Dispose();


                    


                    query = "delete from DKTRUNGBAY_KHACHHANG where DKTRUNGBAY_FK in (SELECT PK_SEQ FROM DANGKYTRUNGBAY WHERE CTTRUNGBAY_FK = '" + cttbId + "') and KHACHHANG_FK = '" + khId + "' and DDKD_FK = " + ddkdId;
                    db.updateQuery(query);

                    query = "insert into DKTRUNGBAY_KHACHHANG(DKTRUNGBAY_FK, KHACHHANG_FK, DANGKY, ImageFilePath, DAI, RONG, CAO, DDKD_FK, THOIDIEM, isPDA) " +
                            "values((SELECT PK_SEQ FROM DANGKYTRUNGBAY WHERE CTTRUNGBAY_FK = '" + cttbId + "'), " + khId + ", " + soxuat + ", '" + fileName + "', " + dai + ", " + rong + ", " + cao + ", " + ddkdId + ", dbo.GetLocalDate(DEFAULT), 1)";

                    if (db.updateQueryReturnInt(query) != 1)
                    {
                        dong["RESULT"] = "0";
                        dong["MSG"] = "3.Xảy ra lỗi khi đăng ký chương trình trưng bày";
                        return dtResult;
                    }
                }
            }
            catch (Exception ex)
            {
                dong["RESULT"] = "0";
                dong["MSG"] = "4.Xảy ra lỗi khi đăng ký" + khId + " (" + ex.Message + ")";
                db.CLose_Connection();
            }
            finally
            {
                db.CLose_Connection();
            }

            return dtResult;
        }
        public static DataTable KhachHang_ChupAnh(string khId, string chupAnh1, byte[] hinhAnhBytes1, string chupAnh2, String hinhAnhBytes2, string ngay, string ddkdId, string cttbId, string nppId)
        {
            DataTable dtResult = new DataTable("ChupAnh");
            dtResult.Columns.Add(new DataColumn("RESULT", Type.GetType("System.String")));
            dtResult.Columns.Add(new DataColumn("MSG", Type.GetType("System.String")));

            DataRow dong = dtResult.NewRow();
            dong["RESULT"] = "1";
            dong["MSG"] = "Lưu thành công!";
            dtResult.Rows.Add(dong);

            if (khId == null || khId.Length <= 0)
            {
                dong["RESULT"] = "0";
                dong["MSG"] = "Dữ liệu truyền (id khách hàng) không đúng định dạng!";
                return dtResult;
            }


            



            if (chupAnh1 == null) chupAnh1 = "0";
            if (chupAnh2 == null) chupAnh2 = "0";

            clsDB db = null;
            try
            {
                db = new clsDB();
                db.BeginTran();
                string today = getDateTime();
                string query = "SELECT count(*) as RD FROM DDKD_KHACHHANG WHERE REPLACE(CONVERT(NVARCHAR(10), THOIDIEM , 102) , '.', '-' ) = '" + today + "' and THOIDIEMDI is not null  and  DDKD_FK='" + ddkdId + "' and khachhang_fk='" + khId + "'";
                int sodr = db.getFirsIntValueSqlCatchException(query);
                //if (sodr > 0)
                //{
                //    db.RollbackAndDispose();
                //    dong["RESULT"] = "0";
                //    dong["MSG"] = "Khách hàng này đã rời đi, Vui lòng chụp hình khách hàng khác! ";
                //    return dtResult;
                //}

                query = " SELECT CASE WHEN LEN(ISNULL(ANHCUAHANG, '')) > 0 THEN 1 ELSE 0 END AS DACHUPANH FROM KHACHHANG WHERE PK_SEQ = " + khId;
                string daChupAnh = db.getFirstStringValueSqlCatchException(query);

                int numRowsAffected = 0;

                //Chụp ảnh đại diện cho cửa hàng (lần đầu tiên)
                if (daChupAnh.Equals("0") && chupAnh1.Equals("1"))
                {
                    string date = XuLy.getDateTime();
                    MemoryStream ms = new MemoryStream(hinhAnhBytes1);
                    string filePath = ConfigurationManager.AppSettings["FilePath"];
                    string fileName = "chupanhKH" + khId + "_" + "_" + date + ".jpg";
                    string fullPath = filePath + "AnhDaiDien\\" + fileName;
                    FileStream fs = new FileStream(fullPath, FileMode.Create, System.IO.FileAccess.Write);

                    ms.WriteTo(fs);
                    ms.Close();
                    fs.Close();
                    ms.Dispose();
                    fs.Dispose();




                    query = "UPDATE KHACHHANG SET ANHCUAHANG = N'" + fileName + "' WHERE PK_SEQ = " + khId;

                    numRowsAffected = db.updateQueryReturnInt(query);
                    if (numRowsAffected <= 0)
                    {
                        db.RollbackAndDispose();
                        dong["RESULT"] = "0";
                        dong["MSG"] = "Xảy ra lỗi khi lưu hình ảnh đại diện!";
                        return dtResult;
                    }
                }

                //Chụp ảnh hàng ngày
                if (chupAnh2.Equals("1"))
                {
                    ngay = getDateTime();





                    string filePath2 = ConfigurationManager.AppSettings["FilePath"];
                    string fileName2 = "anh_" + khId + "_hangngay_" + ngay + ".jpg";
                    string fullPath2 = filePath2 + "anhhangngay\\" + fileName2;
                    MemoryStream ms2 = new MemoryStream(Convert.FromBase64String(hinhAnhBytes2.Replace("\r\n", "")));
                    
                    FileStream fs = new FileStream(fullPath2, FileMode.Create, System.IO.FileAccess.Write);

                    ms2.WriteTo(fs);
                    ms2.Close();
                    fs.Close();
                    ms2.Dispose();
                    fs.Dispose();

                    


                    query = "select  top 1 case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end DuyetBanTraiTuyen,NgayID,PK_SEQ,"
                    + "\n	(select top 1 TANSO  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "'   ) as tanso,"
                    + "\n	(select top 1 SOTT  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "' ) as SOTT"
                    + "\n	from tuyenbanhang tbh "
                    + "\n	where ddkd_fk = '" + ddkdId + "' and npp_fk = '" + nppId + "' "
                    + "\n	and pk_seq in (select tbh_fk from khachhang_tuyenbh where khachhang_fk ='" + khId + "')  "
                    + "\n    order by case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end desc";

                    DataTable dtTuyen = db.getDataTable(query);
                    double DuyetBanTraiTuyen = 0;
                    string tbhId = "null";
                    string ngayid = "null";
                    string stt = "null";
                    string tanso = "null";
                    if (dtTuyen.Rows.Count > 0)
                    {
                        DuyetBanTraiTuyen = double.Parse(dtTuyen.Rows[0]["DuyetBanTraiTuyen"].ToString());
                        tbhId = dtTuyen.Rows[0]["PK_SEQ"].ToString();
                        ngayid = dtTuyen.Rows[0]["NgayID"].ToString();
                        stt = dtTuyen.Rows[0]["SOTT"].ToString();
                        tanso = dtTuyen.Rows[0]["tanso"].ToString();

                    }

                    //Cập nhật database
                    query = "DELETE FROM KHACHHANG_ANHCHUP WHERE KH_FK ='" + khId + "' AND DDKD_FK='" + ddkdId + "' AND NPP_FK='" + nppId + "' AND NGAY='" + ngay + "' ";
                    db.updateQuery(query);
                    query = "INSERT KHACHHANG_ANHCHUP(KHACHHANG_FK, NGAY, FILENAME,DDKD_FK,NPP_FK,isdungtuyen,TBH_FK,NgayId,TANSO,SOTT) " +
                        "SELECT " + khId + ", '" + ngay + "', N'" + fileName2 + "','" + ddkdId + "','" + nppId + "'," + DuyetBanTraiTuyen + "," + tbhId + "," + ngayid + ",'" + tanso + "'," + stt + "";
                
                    numRowsAffected = db.updateQueryReturnInt(query);
                    if (numRowsAffected <= 0)
                    {
                        db.RollbackAndDispose();
                        dong["RESULT"] = "0";
                        dong["MSG"] = "Lưu hình ảnh khách hàng thất bại! - "  +query;
                        return dtResult;
                    }

                }
                db.CommitAndDispose();
                dong["RESULT"] = "1";
                dong["MSG"] = "Lưu hình ảnh thành công!";
                return dtResult;
            }
            catch (Exception ex)
            {
                db.RollbackAndDispose();
                dong["RESULT"] = "0";
                dong["MSG"] = "Xảy ra lỗi khi lưu hình ảnh khách hàng " + khId + " (" + ex.Message + ")";
                return dtResult;
            }



        }



        public static DataTable KhachHang_ChupAnhMerchandising(string khId, string chupAnh1, byte[] hinhAnhBytes1, string chupAnh2, String hinhAnhBytes2, string chupAnh3, string hinhAnhBytes3, string chupAnh4, string hinhAnhBytes4, string chupAnh5, string hinhAnhBytes5, string chupAnh6, string hinhAnhBytes6, string ngay, string ddkdId, string cttbId, string nppId)
        {
            DataTable dtResult = new DataTable("ChupAnh");
            dtResult.Columns.Add(new DataColumn("RESULT", Type.GetType("System.String")));
            dtResult.Columns.Add(new DataColumn("MSG", Type.GetType("System.String")));

            DataRow dong = dtResult.NewRow();
            dong["RESULT"] = "1";
            dong["MSG"] = "Lưu thành công!";
            dtResult.Rows.Add(dong);

            if (khId == null || khId.Length <= 0)
            {
                dong["RESULT"] = "0";
                dong["MSG"] = "Dữ liệu truyền (id khách hàng) không đúng định dạng!";
                return dtResult;
            }






            if (chupAnh1 == null) chupAnh1 = "0";
            if (chupAnh2 == null) chupAnh2 = "0";
            if (chupAnh3 == null) chupAnh3 = "0";
            if (chupAnh4 == null) chupAnh4 = "0";
            if (chupAnh5 == null) chupAnh5 = "0";
            if (chupAnh6 == null) chupAnh6 = "0";

            clsDB db = null;
            try
            {
                db = new clsDB();
                db.BeginTran();
                string today = getDateTime();
                string query = "SELECT count(*) as RD FROM DDKD_KHACHHANG WHERE REPLACE(CONVERT(NVARCHAR(10), THOIDIEM , 102) , '.', '-' ) = '" + today + "' and THOIDIEMDI is not null  and  DDKD_FK='" + ddkdId + "' and khachhang_fk='" + khId + "'";
                int sodr = db.getFirsIntValueSqlCatchException(query);
                if (sodr > 0)
                {
                    db.RollbackAndDispose();
                    dong["RESULT"] = "0";
                    dong["MSG"] = "Khách hàng này đã rời đi, Vui lòng chụp hình khách hàng khác! ";
                    return dtResult;
                }

                query = " SELECT CASE WHEN LEN(ISNULL(ANHCUAHANG, '')) > 0 THEN 1 ELSE 0 END AS DACHUPANH FROM KHACHHANG WHERE PK_SEQ = " + khId;
                string daChupAnh = db.getFirstStringValueSqlCatchException(query);

                int numRowsAffected = 0;

                //Chụp ảnh đại diện cho cửa hàng (lần đầu tiên)
                if (daChupAnh.Equals("0") && chupAnh1.Equals("1"))
                {
                    
                }

                //Chụp ảnh hàng ngày
                if (chupAnh2.Equals("1"))
                {
                    ngay = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;





                    string filePath2 = ConfigurationManager.AppSettings["FilePath"];
                    string fileName2 = "anh_1" + khId + "_hangngay_merchandising" + ngay + ".jpg";
                    string fullPath2 = filePath2 + "anhhangngay_merchandising\\" + fileName2;
                    MemoryStream ms2 = new MemoryStream(Convert.FromBase64String(hinhAnhBytes2.Replace("\r\n", "")));

                    FileStream fs = new FileStream(fullPath2, FileMode.Create, System.IO.FileAccess.Write);

                    ms2.WriteTo(fs);
                    ms2.Close();
                    fs.Close();
                    ms2.Dispose();
                    fs.Dispose();




                    query = "select  top 1 case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end DuyetBanTraiTuyen,NgayID,PK_SEQ,"
                    + "\n	(select top 1 TANSO  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "'   ) as tanso,"
                    + "\n	(select top 1 SOTT  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "' ) as SOTT"
                    + "\n	from tuyenbanhang tbh "
                    + "\n	where ddkd_fk = '" + ddkdId + "' and npp_fk = '" + nppId + "' "
                    + "\n	and pk_seq in (select tbh_fk from khachhang_tuyenbh where khachhang_fk ='" + khId + "')  "
                    + "\n    order by case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end desc";

                    DataTable dtTuyen = db.getDataTable(query);
                    double DuyetBanTraiTuyen = 0;
                    string tbhId = "null";
                    string ngayid = "null";
                    string stt = "null";
                    string tanso = "null";
                    if (dtTuyen.Rows.Count > 0)
                    {
                        DuyetBanTraiTuyen = double.Parse(dtTuyen.Rows[0]["DuyetBanTraiTuyen"].ToString());
                        tbhId = dtTuyen.Rows[0]["PK_SEQ"].ToString();
                        ngayid = dtTuyen.Rows[0]["NgayID"].ToString();
                        stt = dtTuyen.Rows[0]["SOTT"].ToString();
                        tanso = dtTuyen.Rows[0]["tanso"].ToString();

                    }
                    stt=db.getFirstStringValueSqlCatchException("select count(*)+1 from KHACHHANG_ANHCHUP_MERCHANDISING where KH_FK='"+khId+"' and ddkd_fk='"+ddkdId+"' and ngay='"+today+"'");

                    query = "INSERT [KHACHHANG_ANHCHUP_MERCHANDISING](KH_FK, NGAY, FILENAME,DDKD_FK,NPP_FK,isdungtuyen,TBH_FK,NgayId,TANSO,SOTT) " +
                        "SELECT " + khId + ", '" + XuLy.getDateTime() + "', N'" + fileName2 + "','" + ddkdId + "','" + nppId + "'," + DuyetBanTraiTuyen + "," + tbhId + "," + ngayid + ",'" + tanso + "'," + stt + "";

                    numRowsAffected = db.updateQueryReturnInt(query);
                    if (numRowsAffected <= 0)
                    {
                        db.RollbackAndDispose();
                        dong["RESULT"] = "0";
                        dong["MSG"] = "Lưu hình ảnh khách hàng thất bại! - ";
                        return dtResult;
                    }

                }


                //Chụp ảnh hàng ngày
                chupAnh2 = chupAnh3;
                hinhAnhBytes2 = hinhAnhBytes3;

                if (chupAnh2.Equals("1"))
                {
                    ngay = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;

                    string filePath2 = ConfigurationManager.AppSettings["FilePath"];
                    string fileName2 = "anh_2" + khId + "_hangngay_merchandising" + ngay + ".jpg";
                    string fullPath2 = filePath2 + "anhhangngay_merchandising\\" + fileName2;
                    MemoryStream ms2 = new MemoryStream(Convert.FromBase64String(hinhAnhBytes2.Replace("\r\n", "")));

                    FileStream fs = new FileStream(fullPath2, FileMode.Create, System.IO.FileAccess.Write);

                    ms2.WriteTo(fs);
                    ms2.Close();
                    fs.Close();
                    ms2.Dispose();
                    fs.Dispose();

                    query = "select  top 1 case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end DuyetBanTraiTuyen,NgayID,PK_SEQ,"
                    + "\n	(select top 1 TANSO  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "'   ) as tanso,"
                    + "\n	(select top 1 SOTT  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "' ) as SOTT"
                    + "\n	from tuyenbanhang tbh "
                    + "\n	where ddkd_fk = '" + ddkdId + "' and npp_fk = '" + nppId + "' "
                    + "\n	and pk_seq in (select tbh_fk from khachhang_tuyenbh where khachhang_fk ='" + khId + "')  "
                    + "\n    order by case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end desc";

                    DataTable dtTuyen = db.getDataTable(query);
                    double DuyetBanTraiTuyen = 0;
                    string tbhId = "null";
                    string ngayid = "null";
                    string stt = "null";
                    string tanso = "null";
                    if (dtTuyen.Rows.Count > 0)
                    {
                        DuyetBanTraiTuyen = double.Parse(dtTuyen.Rows[0]["DuyetBanTraiTuyen"].ToString());
                        tbhId = dtTuyen.Rows[0]["PK_SEQ"].ToString();
                        ngayid = dtTuyen.Rows[0]["NgayID"].ToString();
                        stt = dtTuyen.Rows[0]["SOTT"].ToString();
                        tanso = dtTuyen.Rows[0]["tanso"].ToString();

                    }
                    stt = db.getFirstStringValueSqlCatchException("select count(*)+1 from KHACHHANG_ANHCHUP_MERCHANDISING where KH_FK='" + khId + "' and ddkd_fk='" + ddkdId + "' and ngay='" + today + "'");

                    query = "INSERT KHACHHANG_ANHCHUP_MERCHANDISING(KH_FK, NGAY, FILENAME,DDKD_FK,NPP_FK,isdungtuyen,TBH_FK,NgayId,TANSO,SOTT) " +
                        "SELECT " + khId + ", '" + XuLy.getDateTime() + "', N'" + fileName2 + "','" + ddkdId + "','" + nppId + "'," + DuyetBanTraiTuyen + "," + tbhId + "," + ngayid + ",'" + tanso + "'," + stt + "";

                    numRowsAffected = db.updateQueryReturnInt(query);
                    if (numRowsAffected <= 0)
                    {
                        db.RollbackAndDispose();
                        dong["RESULT"] = "0";
                        dong["MSG"] = "Lưu hình ảnh khách hàng thất bại! - ";// +query;
                        return dtResult;
                    }

                }

                //Chụp ảnh hàng ngày
                chupAnh2 = chupAnh4;
                hinhAnhBytes2 = hinhAnhBytes4;

                if (chupAnh2.Equals("1"))
                {
                    ngay = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;

                    string filePath2 = ConfigurationManager.AppSettings["FilePath"];
                    string fileName2 = "anh_3" + khId + "_hangngay_merchandising" + ngay + ".jpg";
                    string fullPath2 = filePath2 + "anhhangngay_merchandising\\" + fileName2;
                    MemoryStream ms2 = new MemoryStream(Convert.FromBase64String(hinhAnhBytes2.Replace("\r\n", "")));

                    FileStream fs = new FileStream(fullPath2, FileMode.Create, System.IO.FileAccess.Write);

                    ms2.WriteTo(fs);
                    ms2.Close();
                    fs.Close();
                    ms2.Dispose();
                    fs.Dispose();

                    query = "select  top 1 case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end DuyetBanTraiTuyen,NgayID,PK_SEQ,"
                    + "\n	(select top 1 TANSO  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "'   ) as tanso,"
                    + "\n	(select top 1 SOTT  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "' ) as SOTT"
                    + "\n	from tuyenbanhang tbh "
                    + "\n	where ddkd_fk = '" + ddkdId + "' and npp_fk = '" + nppId + "' "
                    + "\n	and pk_seq in (select tbh_fk from khachhang_tuyenbh where khachhang_fk ='" + khId + "')  "
                    + "\n    order by case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end desc";

                    DataTable dtTuyen = db.getDataTable(query);
                    double DuyetBanTraiTuyen = 0;
                    string tbhId = "null";
                    string ngayid = "null";
                    string stt = "null";
                    string tanso = "null";
                    if (dtTuyen.Rows.Count > 0)
                    {
                        DuyetBanTraiTuyen = double.Parse(dtTuyen.Rows[0]["DuyetBanTraiTuyen"].ToString());
                        tbhId = dtTuyen.Rows[0]["PK_SEQ"].ToString();
                        ngayid = dtTuyen.Rows[0]["NgayID"].ToString();
                        stt = dtTuyen.Rows[0]["SOTT"].ToString();
                        tanso = dtTuyen.Rows[0]["tanso"].ToString();

                    }

                    stt = db.getFirstStringValueSqlCatchException("select count(*)+1 from KHACHHANG_ANHCHUP_MERCHANDISING where KH_FK='" + khId + "' and ddkd_fk='" + ddkdId + "' and ngay='" + today + "'");
                    query = "INSERT KHACHHANG_ANHCHUP_MERCHANDISING(KH_FK, NGAY, FILENAME,DDKD_FK,NPP_FK,isdungtuyen,TBH_FK,NgayId,TANSO,SOTT) " +
                        "SELECT " + khId + ", '" + XuLy.getDateTime() + "', N'" + fileName2 + "','" + ddkdId + "','" + nppId + "'," + DuyetBanTraiTuyen + "," + tbhId + "," + ngayid + ",'" + tanso + "'," + stt + "";

                    numRowsAffected = db.updateQueryReturnInt(query);
                    if (numRowsAffected <= 0)
                    {
                        db.RollbackAndDispose();
                        dong["RESULT"] = "0";
                        dong["MSG"] = "Lưu hình ảnh khách hàng thất bại! - ";// +query;
                        return dtResult;
                    }

                }


                //Chụp ảnh hàng ngày
                chupAnh2 = chupAnh5;
                hinhAnhBytes2 = hinhAnhBytes5;

                if (chupAnh2.Equals("1"))
                {
                    ngay = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;

                    string filePath2 = ConfigurationManager.AppSettings["FilePath"];
                    string fileName2 = "anh_4" + khId + "_hangngay_merchandising" + ngay + ".jpg";
                    string fullPath2 = filePath2 + "anhhangngay_merchandising\\" + fileName2;
                    MemoryStream ms2 = new MemoryStream(Convert.FromBase64String(hinhAnhBytes2.Replace("\r\n", "")));

                    FileStream fs = new FileStream(fullPath2, FileMode.Create, System.IO.FileAccess.Write);

                    ms2.WriteTo(fs);
                    ms2.Close();
                    fs.Close();
                    ms2.Dispose();
                    fs.Dispose();

                    query = "select  top 1 case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end DuyetBanTraiTuyen,NgayID,PK_SEQ,"
                    + "\n	(select top 1 TANSO  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "'   ) as tanso,"
                    + "\n	(select top 1 SOTT  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "' ) as SOTT"
                    + "\n	from tuyenbanhang tbh "
                    + "\n	where ddkd_fk = '" + ddkdId + "' and npp_fk = '" + nppId + "' "
                    + "\n	and pk_seq in (select tbh_fk from khachhang_tuyenbh where khachhang_fk ='" + khId + "')  "
                    + "\n    order by case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end desc";

                    DataTable dtTuyen = db.getDataTable(query);
                    double DuyetBanTraiTuyen = 0;
                    string tbhId = "null";
                    string ngayid = "null";
                    string stt = "null";
                    string tanso = "null";
                    if (dtTuyen.Rows.Count > 0)
                    {
                        DuyetBanTraiTuyen = double.Parse(dtTuyen.Rows[0]["DuyetBanTraiTuyen"].ToString());
                        tbhId = dtTuyen.Rows[0]["PK_SEQ"].ToString();
                        ngayid = dtTuyen.Rows[0]["NgayID"].ToString();
                        stt = dtTuyen.Rows[0]["SOTT"].ToString();
                        tanso = dtTuyen.Rows[0]["tanso"].ToString();

                    }
                    stt = db.getFirstStringValueSqlCatchException("select count(*)+1 from KHACHHANG_ANHCHUP_MERCHANDISING where KH_FK='" + khId + "' and ddkd_fk='" + ddkdId + "' and ngay='" + today + "'");

                    query = "INSERT KHACHHANG_ANHCHUP_MERCHANDISING(KH_FK, NGAY, FILENAME,DDKD_FK,NPP_FK,isdungtuyen,TBH_FK,NgayId,TANSO,SOTT) " +
                        "SELECT " + khId + ", '" + XuLy.getDateTime() + "', N'" + fileName2 + "','" + ddkdId + "','" + nppId + "'," + DuyetBanTraiTuyen + "," + tbhId + "," + ngayid + ",'" + tanso + "'," + stt + "";

                    numRowsAffected = db.updateQueryReturnInt(query);
                    if (numRowsAffected <= 0)
                    {
                        db.RollbackAndDispose();
                        dong["RESULT"] = "0";
                        dong["MSG"] = "Lưu hình ảnh khách hàng thất bại! - ";// +query;
                        return dtResult;
                    }

                }


                //Chụp ảnh hàng ngày
                chupAnh2 = chupAnh6;
                hinhAnhBytes2 = hinhAnhBytes6;

                if (chupAnh2.Equals("1"))
                {
                    ngay = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;

                    string filePath2 = ConfigurationManager.AppSettings["FilePath"];
                    string fileName2 = "anh_5" + khId + "_hangngay_merchandising" + ngay + ".jpg";
                    string fullPath2 = filePath2 + "anhhangngay_merchandising\\" + fileName2;
                    MemoryStream ms2 = new MemoryStream(Convert.FromBase64String(hinhAnhBytes2.Replace("\r\n", "")));

                    FileStream fs = new FileStream(fullPath2, FileMode.Create, System.IO.FileAccess.Write);

                    ms2.WriteTo(fs);
                    ms2.Close();
                    fs.Close();
                    ms2.Dispose();
                    fs.Dispose();

                    query = "select  top 1 case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end DuyetBanTraiTuyen,NgayID,PK_SEQ,"
                    + "\n	(select top 1 TANSO  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "'   ) as tanso,"
                    + "\n	(select top 1 SOTT  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "' ) as SOTT"
                    + "\n	from tuyenbanhang tbh "
                    + "\n	where ddkd_fk = '" + ddkdId + "' and npp_fk = '" + nppId + "' "
                    + "\n	and pk_seq in (select tbh_fk from khachhang_tuyenbh where khachhang_fk ='" + khId + "')  "
                    + "\n    order by case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end desc";

                    DataTable dtTuyen = db.getDataTable(query);
                    double DuyetBanTraiTuyen = 0;
                    string tbhId = "null";
                    string ngayid = "null";
                    string stt = "null";
                    string tanso = "null";
                    if (dtTuyen.Rows.Count > 0)
                    {
                        DuyetBanTraiTuyen = double.Parse(dtTuyen.Rows[0]["DuyetBanTraiTuyen"].ToString());
                        tbhId = dtTuyen.Rows[0]["PK_SEQ"].ToString();
                        ngayid = dtTuyen.Rows[0]["NgayID"].ToString();
                        stt = dtTuyen.Rows[0]["SOTT"].ToString();
                        tanso = dtTuyen.Rows[0]["tanso"].ToString();

                    }
                    stt = db.getFirstStringValueSqlCatchException("select count(*)+1 from KHACHHANG_ANHCHUP_MERCHANDISING where KH_FK='" + khId + "' and ddkd_fk='" + ddkdId + "' and ngay='" + today + "'");

                    query = "INSERT KHACHHANG_ANHCHUP_MERCHANDISING(KH_FK, NGAY, FILENAME,DDKD_FK,NPP_FK,isdungtuyen,TBH_FK,NgayId,TANSO,SOTT) " +
                        "SELECT " + khId + ", '" + XuLy.getDateTime() + "', N'" + fileName2 + "','" + ddkdId + "','" + nppId + "'," + DuyetBanTraiTuyen + "," + tbhId + "," + ngayid + ",'" + tanso + "'," + stt + "";

                    numRowsAffected = db.updateQueryReturnInt(query);
                    if (numRowsAffected <= 0)
                    {
                        db.RollbackAndDispose();
                        dong["RESULT"] = "0";
                        dong["MSG"] = "Lưu hình ảnh khách hàng thất bại! - ";// +query;
                        return dtResult;
                    }

                }



                db.CommitAndDispose();
                dong["RESULT"] = "1";
                dong["MSG"] = "Lưu hình ảnh thành công!";
                return dtResult;
            }
            catch (Exception ex)
            {
                db.RollbackAndDispose();
                dong["RESULT"] = "0";
                dong["MSG"] = "Xảy ra lỗi khi lưu hình ảnh khách hàng " + khId + " (" + ex.Message + ")";
                return dtResult;
            }



        }

        internal static string ifNull(string barcode_fk)
        {
            try
            {
                if (decimal.Parse(barcode_fk) > 0)
                {
                    return barcode_fk.ToString();
                }
                else
                {
                    return "NULL";
                }
            }
            catch { return "NULL"; };
        }

        public static string KeHoach_ChuphinhNpp(string nvId, string Id, String hinhAnhBytes2)
        {
            DataTable dtResult = new DataTable("ChupAnh");
            dtResult.Columns.Add(new DataColumn("RESULT", Type.GetType("System.String")));
            dtResult.Columns.Add(new DataColumn("MSG", Type.GetType("System.String")));

            DataRow dong = dtResult.NewRow();
            dong["RESULT"] = "1";
            dong["MSG"] = "Lưu thành công!";
            dtResult.Rows.Add(dong);
            string fileName = "";
            clsDB db = null;
            string query = "";
            string msg = "";

            try
            {
                db = new clsDB();
                string ngay = getDateTime();
                string filePath = ConfigurationManager.AppSettings["FilePath"];
                 fileName = "anh_KeHoach_ChuphinhNpp" + nvId + "_hangngay_" + ngay + ".jpg";
                string fullPath = filePath + fileName;
                MemoryStream ms2 = new MemoryStream(Convert.FromBase64String(hinhAnhBytes2.Replace("\r\n", "")));
                UploadToAzure(ms2, "anhhangngay", fullPath);
                ms2.Close();
                ms2.Dispose();

              
                query = "UPDATE KEHOACHNV_NPP SET PATHIMAGE = N'" + fileName + "',THOIDIEMCHUPANH = getdate() WHERE PK_SEQ = '" + Id + "'";
                int numRowsAffected = db.updateQueryReturnInt(query);
                if (numRowsAffected <= 0)
                {
                    db.RollbackAndDispose();
                    dong["RESULT"] = "0";
                    dong["MSG"] = "Xảy ra lỗi khi lưu hình tổng quan!  " + query;
                    return ParseDataTableToJSon( dtResult);
                }
                return ParseDataTableToJSon(dtResult);
            }
            catch (Exception ex)
            {
                dong["RESULT"] = "0";
                dong["MSG"] = "Xảy ra lỗi hệ thông khi lưu hình" + ex.ToString() + msg;
                db.CLose_Connection();
                return ParseDataTableToJSon(dtResult);
            }



        }
        public static string KeHoach_ChuphinhKhachhang(string nhanvienId, string khachhang_fk, string kehoach_fk, String image)
        {
            DataTable dtResult = new DataTable("ChupAnh");
            dtResult.Columns.Add(new DataColumn("RESULT", Type.GetType("System.String")));
            dtResult.Columns.Add(new DataColumn("MSG", Type.GetType("System.String")));

            DataRow dong = dtResult.NewRow();
            dong["RESULT"] = "1";
            dong["MSG"] = "Lưu thành công!";
            dtResult.Rows.Add(dong);

            string ngayHienTai = (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());
            string thangHienTai = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            int namHienTai = DateTime.Now.Year;
            string thangnam = namHienTai + "-" + thangHienTai + "-" + ngayHienTai;

            string query = "SELECT KBH_FK FROM GIAMSATBANHANG where KBH_FK != 100043 and PK_SEQ = (select GSBH_FK from NHANVIEN WHERE PK_SEQ = '" + nhanvienId + "')";
            DataTable KBH = new DataTable("KBH");
            clsDB db = null;
            db = new clsDB();
            if (db.getFirsIntValueSqlCatchException(query) > 0)
            {
                DataTable dark = new DataTable("ROIKHOI");
                string sql = "SELECT THOIDIEMDI FROM ddkd_khachhang " +
                             "  WHERE ddkd_fk = (SELECT DDKD_FK FROM KEHOACHNV_TBH WHERE PK_SEQ = '" + kehoach_fk + "') " +
                             "  AND khachhang_fk = '" + khachhang_fk + "' AND CONVERT(VARCHAR(10),THOIDIEMDI, 126) = '" + thangnam + "'";
                //da = new SqlDataAdapter(sql, chuoiKN);
               // da.Fill(dark);
               // da.Dispose();
                if (db.getFirsIntValueSqlCatchException(sql) > 0)
                {
                    dong["RESULT"] = "0";
                    dong["MSG"] = "Nhân viên bán hàng đã rời khỏi cửa hàng, bạn không thể chụp ảnh.";
                    return XuLy.ParseDataTableToJSon(dtResult);
                }
            }


           // ketNoi = new SqlConnection(chuoiKN);

            try
            {
                // if (ketNoi.State == ConnectionState.Closed)
                // {
                //     ketNoi.Open();
                //  }

                db = new clsDB();
                string ngay = getDateTime();
                string filePath = ConfigurationManager.AppSettings["FilePath"];
               string fileName = "anh_KeHoach_ChuphinhNv" + nhanvienId + "_hangngay_" + ngay + ".jpg";
                string fullPath = filePath + fileName;
                MemoryStream ms2 = new MemoryStream(Convert.FromBase64String(image.Replace("\r\n", "")));
                UploadToAzure(ms2, "anhhangngay", fullPath);
                ms2.Close();
                ms2.Dispose();
                String timeNow = DateTime.Now.ToString();
                query = "UPDATE KEHOACHNV_TBH_KHACHHANG SET IMAGEPATH = '" + fileName + "' WHERE KHACHHANG_FK = '" + khachhang_fk + "' AND KEHOACHNV_TBH_FK = '" + kehoach_fk + "'";
                
                int count = db.updateQueryReturnInt(query);

                if (count <= 0)
                {
                    dong["RESULT"] = "0";
                    dong["MSG"] = "Xảy ra lỗi khi lưu hình ảnh! "+query;
                    return XuLy.ParseDataTableToJSon(dtResult);
                }


            }
            catch (Exception ex)
            {
                dong["RESULT"] = "0";
                dong["MSG"] = "Xảy ra lỗi khi lưu hình ảnh";
            }
           
            return XuLy.ParseDataTableToJSon(dtResult);



        }


        public static DataTable KhachHang_ChupAnh_buoc3(string khId, string chupAnh1, String hinhAnhBytes1, string chupAnh2, String hinhAnhBytes2, string ngay, string ddkdId, string cttbId, string nppId)
        {
            DataTable dtResult = new DataTable("ChupAnh");
            dtResult.Columns.Add(new DataColumn("RESULT", Type.GetType("System.String")));
            dtResult.Columns.Add(new DataColumn("MSG", Type.GetType("System.String")));

            DataRow dong = dtResult.NewRow();
            dong["RESULT"] = "1";
            dong["MSG"] = "Lưu thành công!";
            dtResult.Rows.Add(dong);

            if (khId == null || khId.Length <= 0)
            {
                dong["RESULT"] = "0";
                dong["MSG"] = "Dữ liệu truyền (id khách hàng) không đúng định dạng!";
                return dtResult;
            }

            if (chupAnh1 == null) chupAnh1 = "1";
            if (chupAnh2 == null) chupAnh2 = "0";

            clsDB db = null;
            try
            {
                db = new clsDB();
                db.BeginTran();
                string today = getDateTime();
                ngay = today;


                string query = "SELECT count(*) as RD FROM DDKD_KHACHHANG WHERE REPLACE(CONVERT(NVARCHAR(10), THOIDIEM , 102) , '.', '-' ) = '" + today + "' and THOIDIEMDI is not null  and  DDKD_FK='" + ddkdId + "' and khachhang_fk='" + khId + "'";
                int sodr = db.getFirsIntValueSqlCatchException(query);
               // if (sodr > 0)
               // {
               /////     db.RollbackAndDispose();
               //     dong["RESULT"] = "0";
               //     dong["MSG"] = "Khách hàng này đã rời đi, Vui lòng chụp hình khách hàng khác! ";
               //     return dtResult;
               //  }



                query = "select  top 1 case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end DuyetBanTraiTuyen,NgayID,PK_SEQ,"
                   + "\n	(select top 1 TANSO  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "'   ) as tanso,"
                   + "\n	(select top 1 SOTT  from KHACHHANG_TUYENBH where TBH_FK=tbh.PK_SEQ and KHACHHANG_FK='" + khId + "' ) as SOTT"
                   + "\n	from tuyenbanhang tbh "
                   + "\n	where ddkd_fk = '" + ddkdId + "' and npp_fk = '" + nppId + "' "
                   + "\n	and pk_seq in (select tbh_fk from khachhang_tuyenbh where khachhang_fk ='" + khId + "')  "
                   + "\n    order by case NgayID when DATEPART(dw,dbo.GetLocalDate(DEFAULT)) then 1 else 0 end desc";

                DataTable dtTuyen = db.getDataTable(query);
                double DuyetBanTraiTuyen = 0;
                string tbhId = "null";
                string ngayid = "null";
                string stt = "null";
                string tanso = "null";
                if (dtTuyen.Rows.Count > 0)
                {
                    DuyetBanTraiTuyen = double.Parse(dtTuyen.Rows[0]["DuyetBanTraiTuyen"].ToString());
                    tbhId = dtTuyen.Rows[0]["PK_SEQ"].ToString();
                    ngayid = dtTuyen.Rows[0]["NgayID"].ToString();
                    stt = dtTuyen.Rows[0]["SOTT"].ToString();
                    tanso = dtTuyen.Rows[0]["tanso"].ToString();

                }

                if (chupAnh1.Equals("1"))
                {
                    query = " SELECT COUNT(*)a FROM KHACHHANG_ANHCHUP WHERE  KH_FK =  '"+khId+"'  and DDKD_FK = '"+ddkdId+ "'  and anhtongquan = 1 and ngay > '2018-06-11' ";
                    int daChupAnh = db.getFirsIntValueSqlCatchException(query);
                    if (daChupAnh>0) {
                        db.RollbackAndDispose();
                        dong["RESULT"] = "0";
                        dong["MSG"] = "Ảnh tổng quan của khách hàng này đã được chụp ";
                        return dtResult;
                    }
                    else
                    {
                        
                        if (1==1)
                        {
                            string filePath2 = ConfigurationManager.AppSettings["FilePath"];
                            string fileName2 = "anh_tongquan" + khId + "_hangngay_" + ngay + ".jpg";
                            string fullPath2 = filePath2 + fileName2;
                            MemoryStream ms2 = new MemoryStream(Convert.FromBase64String(hinhAnhBytes1.Replace("\r\n", "")));
                            UploadToAzure(ms2, "anhhangngay", fullPath2);
                            ms2.Close();
                            ms2.Dispose();

                            query = " INSERT KHACHHANG_ANHCHUP(KH_FK, FILENAME, DDKD_FK, NPP_FK, isdungtuyen, TBH_FK, NgayId, TANSO, SOTT,anhtongquan,ngay) " +
                                  " SELECT " + khId + ", N'" + fileName2 + "','" + ddkdId + "','" + nppId + "'," + DuyetBanTraiTuyen + "," + tbhId + "," + ngayid + ",'" + tanso + "'," + stt + " ,1,'"+today+"'";

                          int numRowsAffected = db.updateQueryReturnInt(query);
                            if (numRowsAffected <= 0)
                            {
                                db.RollbackAndDispose();
                                dong["RESULT"] = "0";
                                dong["MSG"] = "Xảy ra lỗi khi lưu hình tổng quan!  "+query;
                                return dtResult;
                            }
                        }
                    }
                   


                }
                if (chupAnh2.Equals("1"))
                {
                    query = " SELECT COUNT(*)a FROM KHACHHANG_ANHCHUP WHERE  KH_FK =  '" + khId + "'  and DDKD_FK = '" + ddkdId + "' and anhtbkhuvuc = 1  and ngay > '2018-06-11'";
                    int daChupAnh = db.getFirsIntValueSqlCatchException(query);
                    if (daChupAnh>0)
                    {
                        db.RollbackAndDispose();
                        dong["RESULT"] = "0";
                        dong["MSG"] = "Ảnh TB khu vực của khách hàng này đã được chụp";
                        return dtResult;
                    }
                    else {
                       
                            string filePath2 = ConfigurationManager.AppSettings["FilePath"];
                            string fileName2 = "anh_tb" + khId + "_hangngay_" + ngay + ".jpg";
                            string fullPath2 = filePath2 + fileName2;
                            MemoryStream ms2 = new MemoryStream(Convert.FromBase64String(hinhAnhBytes2.Replace("\r\n", "")));
                            UploadToAzure(ms2, "anhhangngay", fileName2);
                            ms2.Close();
                            ms2.Dispose();


                            query = "INSERT KHACHHANG_ANHCHUP(KH_FK, FILENAME, DDKD_FK, NPP_FK, isdungtuyen, TBH_FK, NgayId, TANSO, SOTT,anhtbkhuvuc,ngay) " +
                                  "SELECT " + khId + ", N'" + fileName2 + "','" + ddkdId + "','" + nppId + "'," + DuyetBanTraiTuyen + "," + tbhId + "," + ngayid + ",'" + tanso + "'," + stt + " ,1,'"+today+"'";

                            int numRowsAffected = db.updateQueryReturnInt(query);
                            if (numRowsAffected <= 0)
                            {
                                db.RollbackAndDispose();
                                dong["RESULT"] = "0";
                                dong["MSG"] = "Xảy ra lỗi khi lưu hình ảnh TB khu vực! "+query;
                                return dtResult;
                            }
                        
                    }
                }


              
               
                db.CommitAndDispose();
                dong["RESULT"] = "1";
                dong["MSG"] = "Lưu hình ảnh thành công!";
                return dtResult;
            }
            catch (Exception ex)
            {
                db.RollbackAndDispose();
                dong["RESULT"] = "0";
                dong["MSG"] = "Xảy ra lỗi khi lưu hình ảnh khách hàng " + khId + " (" + ex.Message + ")";
                return dtResult;
            }

            // bo sung TNI




        }

        public static string DonHang_LuuSanPhamSuDungKhuyenMai(clsDB db, string SanPhamSuDung, string dhId)
        {
            //dkkmLog.pCtkmId + " # " + dkkmLog.pDkkmId + " # " + dkkmLog.pSpList[s].pMa + " # " + soluong + " # " + dongia

            string query = "";

            string[] ctkm = SanPhamSuDung.Split(new[] { " ; " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < ctkm.Length; i++)
            {
                string[] spkm = ctkm[i].Split(new[] { " # " }, StringSplitOptions.RemoveEmptyEntries);
                if(spkm.Length>3)
                {
                    string CtkmId = spkm[0];
                    string DkkmId = spkm[1];
                    string spMa = spkm[2];
                    string soluong = spkm[3];
                    string dongia = spkm[4];

                    string slDonHang = " (  select sum(soluong) from donhang_sanpham where donhang_fk =  " + dhId + " and  sanpham_fk = sp.pk_seq     ) ";

                    query = "\n insert donhang_ctkm_dkkm (soluongdonhang,donhang_fk,ctkm_fk,dkkm_fk,soluongmua,soxuat,sanpham_fk,doanhso) " +
                            "\n select  " + slDonHang + " ," + dhId + "," + CtkmId + "," + DkkmId + "," + soluong + ",0,sp.pk_seq," + dongia + "*" + soluong + " from sanpham sp where sp.ma ='" + spMa + "'   ";
                    if (db.updateQueryReturnInt(query) <= 0)
                    {
                        return "Lỗi lưu su dung KM : " + query;
                    }
                }
                
            }


            return "";
        }

        public static string DonHang_LuuKhuyenMai(clsDB db, String nppId, string khId, string ngaynhap, string dhId, string khuyenmai, ref string thongbao)
        {
            String query = "";
            if (!khuyenmai.Equals(""))
            {
                //ctkmId # trakmId # SoXuat # TongGiaTri # spMa # Sluong
                string[] km = khuyenmai.Split(new char[] { ';' });

                //CHECK TỒN KHO KHUYẾN MÃI
                //bool checkTonKho = true;  //CHUA BIET SO XUAT MOI LA BAO NHIEU NEN VAN PHAI XUONG DUOI MOI CHECK
                //for (int i = 0; i < km.Length; i++)
                //{
                //    string[] khuyen_mai = km[i].Split(new char[] { '#' });

                //    string khoSql = "(SELECT TOP(1) KHO_FK FROM CTKHUYENMAI WHERE SCHEME = '" + khuyen_mai[0] + "')";

                //    string spId = "NULL";
                //    string spSoluong = "NULL";

                //    if (!khuyen_mai[4].Trim().Equals("-1"))
                //    {
                //        spTen = khuyen_mai[4].Trim();
                //        spSoluong = khuyen_mai[5].Trim();

                //        //query = "select top(1) (isnull(available, 0) - " + spSoluong + ") as available from nhapp_kho where sanpham_fk = (select pk_seq from sanpham where ten = N'" + spTen + "') and npp_fk='" + nppId + "' and kho_fk= " + khoSql + " and kbh_fk = '" + _kbh_fk + "' ";
                //        //string ton = db.getFirstStringValueSqlCatchException(query);
                //        //long available = long.Parse(ton);
                //        //if (available <= 0)
                //        //{

                //        //    thongbao += "-" + spTen + "  Tồn kho km không đủ!\n";
                //        //    // break;
                //        //}
                //    }
                //}

                //if (!checkTonKho)
                //{
                //    thongbao = " Tồn kho khuyến mãi không đủ! Không thể lưu khuyến mãi";
                //}
                //else
                {
                    for (int i = 0; i < km.Length; i++)
                    {
                        string[] khuyen_mai = km[i].Split(new char[] { '#' });
                        string khoSql = "(SELECT TOP(1) KHO_FK FROM CTKHUYENMAI WHERE SCHEME = '" + khuyen_mai[0] + "')";
                        string spId = "NULL";
                        string spSoluong = "NULL";

                        if (khuyen_mai[4].Trim().Length > 3)
                        {
                            spId = khuyen_mai[4].Trim();
                            spSoluong = khuyen_mai[5].Trim();
                        }

                        ////
                        if (!spId.Equals("NULL"))
                        {
                            //KHUYẾN MÃI TRẢ SP
                            //Lấy giá sản phẩm trả km
                            string giaSql = "\n select dbo.[GiaBanLeNppSanPham]((select kbh_fk from khachhang where pk_Seq = "+khId+")," + nppId + ","+spId+", '" + ngaynhap + "') ";

                            double dongia = db.getFirstDoubleValueSqlCatchException(giaSql);
                            //Lấy kho theo chương trình khuyến mãi
                            khoSql = "(SELECT TOP(1) KHO_FK FROM CTKHUYENMAI WHERE SCHEME = '" + khuyen_mai[0] + "')";


                            //demo bỏ thật test lại nhá
                            string checkNS_Msg = "";// CheckNganSachKM(db, khuyen_mai[0], nppId, khId, double.Parse(spSoluong), double.Parse(spSoluong) * dongia);
                            // if (checkNS_Msg.Trim().Length > 0)
                            if (false)
                            {
                                thongbao += checkNS_Msg + "\n";
                            }
                            else
                            {
                                query = "insert donhang_ctkm_trakm(sanpham_fk,donhangId, ctkmId, trakmId, soxuat, tonggiatri, spMa, soluong) " +
                                      "select a.pk_seq,'" + dhId + "', b.pk_seq, '" + khuyen_mai[1] + "', '" + khuyen_mai[2] + "', (" + spSoluong + " * " + dongia + "), a.ma, " + spSoluong + " from SanPham a, Ctkhuyenmai b where a.pk_seq ="+spId+" and b.scheme = '" + khuyen_mai[0] + "' ";

                                if (db.updateQueryReturnInt(query) <= 0)
                                {
                                    return "Không thể lưu khuyến mãi '" + khuyen_mai[0] + "' sản phẩm '" + spId + "', số lượng '" + spSoluong + "' của đơn hàng";
                                }

                              

                            }
                        }
                        else  //CHUA CO XAI
                        {
                            string checkNS_Msg = "";// CheckNganSachKM(db, khuyen_mai[0], nppId, khId, 0.0, double.Parse(khuyen_mai[3]));

                           // if (checkNS_Msg.Trim().Length > 0)
                           if(false)
                            {
                                thongbao += checkNS_Msg + "\n";
                            }
                            else
                            {
                                //Khuyến mãi tiền / chiết khấu
                                query = "insert donhang_ctkm_trakm(donhangId, ctkmId, trakmId, soxuat, tonggiatri) " +
                                      "select '" + dhId + "', pk_seq, '" + khuyen_mai[1] + "', '" + khuyen_mai[2] + "', '" + khuyen_mai[3] + "' from Ctkhuyenmai where scheme = '" + khuyen_mai[0] + "' ";
                                if (db.updateQueryReturnInt(query) <= 0)
                                {
                                    return "Không thể lưu khuyến mãi '" + khuyen_mai[0] + "' số tiền '" + khuyen_mai[3] + "' của đơn hàng";
                                }

                            }

                        }


                    }
                }
                //  return "";
            }
            //string msg = dexuatlo_km(db, dhId, ngaynhap, nppId, khId);
            //if (msg.Length > 0) return msg;

            return "";
        }

        public static string CheckNganSachKM(clsDB db, string schemeKm, string nppId, string khId, double soluong, double sotien)
        {

            string sql = "";
            sql = " select level_phanbo from CTKHUYENMAI where scheme = '" + schemeKm + "' ";
            string mucphanbo = db.getFirstStringValueSqlCatchException(sql);
            if (mucphanbo.Trim().Length <= 0)
            {
                return "Khong xac dinh duoc muc phan bo!";
            }
            if (mucphanbo.Equals("0"))
            {
                sql = "select a.ctkm_fk, b.scheme, b.phanbotheodonhang, a.ngansach,  " +
                        "\n	case  " +
                        "\n	when b.phanbotheodonhang=0 then " +
                        "\n			isnull( ( select sum(tonggiatri) " +
                        "\n			  from donhang_ctkm_trakm " +
                        "\n			  where ctkmid = a.ctkm_fk  and donhangid in ( select pk_seq from donhang where npp_fk = a.npp_fk and trangthai != 2 ) and donhangid not in ( select donhang_fk from donhangtrave where donhang_fk is not				null and npp_fk = '" + nppId + "' and trangthai in ( 1, 3 ) ) ), 0 ) " +
                        "\nelse " +
                        "\n	isnull( ( select sum(soluong) " +
                        "\n	from donhang_ctkm_trakm " +
                        "\n	where ctkmid = a.ctkm_fk and spma is not null and donhangid in " +
                        "\n		( select pk_seq from donhang where npp_fk = a.npp_fk and	trangthai != 2 ) and donhangid not in ( select donhang_fk	from donhangtrave where donhang_fk is not null and npp_fk = '" + nppId + "' and trangthai in ( 1, 3 ) ) ), 0 ) end as dasudung " +
                        "\nfrom phanbokhuyenmai a inner join ctkhuyenmai b on a.ctkm_fk = b.pk_seq " +
                        "\nwhere npp_fk = '" + nppId + "' and b.scheme = '" + schemeKm + "'  ";
            }
            else
            {
                sql = "select a.ctkm_fk, b.scheme, b.phanbotheodonhang, a.ngansach,  " +
                        "\n	case  " +
                        "\n	when b.phanbotheodonhang=0 then " +
                        "\n			isnull( ( select sum(tonggiatri) " +
                        "\n			  from donhang_ctkm_trakm " +
                        "\n			  where ctkmid = a.ctkm_fk  and donhangid in ( select pk_seq from donhang where khachhang_fk = '" + khId + "' and trangthai != 2 ) and donhangid not in ( select donhang_fk from donhangtrave where donhang_fk is not null and khachhang_fk = '" + khId + "' and trangthai in ( 1, 3 ) ) ), 0 ) " +
                        "\nelse " +
                        "\n	isnull( ( select sum(soluong) " +
                        "\n	from donhang_ctkm_trakm " +
                        "\n	where ctkmid = a.ctkm_fk and spma is not null and donhangid in " +
                        "\n		( select pk_seq from donhang where khachhang_fk = '" + khId + "' and	trangthai != 2 ) and donhangid not in ( select donhang_fk	from donhangtrave where donhang_fk is not null and khachhang_fk = '" + khId + "' and trangthai in ( 1, 3 ) ) ), 0 ) end as dasudung " +
                        "\nfrom phanbokhuyenmai a inner join ctkhuyenmai b on a.ctkm_fk = b.pk_seq " +
                        "\nwhere khachhang_fk = '" + khId + "' and b.scheme ='" + schemeKm + "'  ";

                //tạm thời ntn 2019-02-18
                sql = "select a.ctkm_fk, b.scheme, b.phanbotheodonhang, a.ngansach,  " +
                        "\n	case  " +
                        "\n	when b.phanbotheodonhang=0 then " +
                        "\n			isnull( ( select sum(tonggiatri) " +
                        "\n			  from donhang_ctkm_trakm " +
                        "\n			  where ctkmid = a.ctkm_fk  and donhangid in ( select pk_seq from donhang where npp_fk = a.npp_fk and trangthai != 2 ) and donhangid not in ( select donhang_fk from donhangtrave where donhang_fk is not				null and npp_fk = '" + nppId + "' and trangthai in ( 1, 3 ) ) ), 0 ) " +
                        "\nelse " +
                        "\n	isnull( ( select sum(soluong) " +
                        "\n	from donhang_ctkm_trakm " +
                        "\n	where ctkmid = a.ctkm_fk and spma is not null and donhangid in " +
                        "\n		( select pk_seq from donhang where npp_fk = a.npp_fk and	trangthai != 2 ) and donhangid not in ( select donhang_fk	from donhangtrave where donhang_fk is not null and npp_fk = '" + nppId + "' and trangthai in ( 1, 3 ) ) ), 0 ) end as dasudung " +
                        "\nfrom phanbokhuyenmai a inner join ctkhuyenmai b on a.ctkm_fk = b.pk_seq " +
                        "\nwhere npp_fk = '" + nppId + "' and b.scheme = '" + schemeKm + "'  ";

            }
            DataTable rs = db.getDataTable(sql);
            string scheme = "";
            try
            {
                int loaipb = 0;
                double conlai = 0.0f;
                if (rs.Rows.Count > 0)
                {
                    scheme = rs.Rows[0]["scheme"].ToString();
                    conlai = double.Parse(rs.Rows[0]["ngansach"].ToString()) - double.Parse(rs.Rows[0]["DASUDUNG"].ToString());
                    loaipb = int.Parse(rs.Rows[0]["phanbotheodonhang"].ToString());
                }
                rs.Dispose();

                if (loaipb == 0)// tongtien
                {
                    if (sotien > conlai)
                    {
                        return "1.Chương trình khuyến mại: " + scheme + ", đã hết ngân sách phân bổ";
                    }
                }
                else// soluong
                {
                    if (soluong > conlai)
                    {
                        return "1.Chương trình khuyến mại: " + scheme + ", đã hết ngân sách phân bổ";
                    }
                }

            }
            catch (Exception e)
            {
                return "2.Chương trình khuyến mại: " + scheme + ", đã hết ngân sách phân bổ-" + e.Message;
            }
            return "";
        }

        //CAP NHAT TONG GIA TRI DON HANG
        public static String Update_GiaTri_DonHang(String dhId, clsDB db)
        {

            try
            {

                String query = "\n  update DONHANG set TONGGIATRI= " +
                                 "\n           round( isnull(( " +
                                 "\n             select SUM(round(b.soluong*b.giamua,0))  as TongGiaTri " +
                                 "\n             from  DONHANG_SANPHAM b where b.DONHANG_FK=DONHANG.PK_SEQ " +
                                 "\n            ),0) - " +
                                 "\n            isnull(( " +
                                 "\n             select SUM(TONGGIATRI)  as TongGiaTri " +
                                 "\n             from DONHANG_CTKM_TRAKM a" +
                                 "\n             where  a.DONHANGID=DONHANG.PK_SEQ and SPMA is null" +
                                 "\n            ),0)  ,0)" +
                                 "\n  " +
                                 "\n  from DONHANG where pk_Seq= " + dhId;

                if (db.updateQueryReturnInt(query)<=0)
                {
                    return "Lỗi phát sinh khi cập nhật giá trị đơn hàng " + query;
                }
                return "";
            }
            catch (Exception ex)
            {
                return "Exception:" + ex.Message;
            }
        }


        public static String sendPost(String url, String urlParameters) // authenticationFAST
        {
            
            try
            {
                

                string result = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";                
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(urlParameters);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    result = responseReader.ReadToEnd();
                }
                return result;

            }
            catch (Exception e)
            {

                return "{\"status\":\"0\",\"msg\":\"Exception:" + e.Message + "\"}";
            }
        }

        public static String sha256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
