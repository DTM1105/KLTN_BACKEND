using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace OneSolution_API.Models.Utils 
{
    public class RemakeClass
    {
        public static string GetJson(DataTable dtResult)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            
            foreach (DataRow row in dtResult.Rows)
            {
                childRow = new Dictionary<string, object>();
                
                foreach (DataColumn col in dtResult.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                
                parentRow.Add(childRow);
            }
            
            return jsSerializer.Serialize(parentRow);
        }

        public static string layngayhientai()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public static int chuyenInt(String intString)
        {
            int i = 0;

            if (!Int32.TryParse(intString, out i))
            {
                i = -1;
            }

            return i;
        }

        public static string laythoigian()
        {
            return DateTime.Now.ToString("HH:MM:ss");
        }

        public static String layngaygiohientai()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
        }

        internal static decimal pasreStringToDeceimal(string v)
        {
            try
            {
                return decimal.Parse(v);
            }
            catch { }
            return 0;
            
        }
    }
}