using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlTypes;
using Newtonsoft.Json;

namespace OneSolution_API.Models.Utils
{

    public class clsDB : IDisposable
    {

        private SqlConnection m_Connection;
       
        SqlTransaction sqlTran;
        public SqlCommand sqlCmd;
        public static string strConnSalesUp = ConfigurationManager.ConnectionStrings["connectionString_2"].ConnectionString;
      //  public static string strConnSync = ConfigurationManager.ConnectionStrings["connectionStringSync"].ConnectionString;

        private string connectionStr = "";

        //Properties.Settings.Default.OPV;
        public clsDB()
        {
            try
            {
                this.pConnectionStr = strConnSalesUp;
                m_Connection = new SqlConnection(this.pConnectionStr);


                if (m_Connection.State == ConnectionState.Closed)
                    m_Connection.Open();
                sqlCmd = new SqlCommand("", m_Connection);
            }
            catch(Exception e)
            {
                string a = e.Message;

            }
           
        }

        //db.sqlCmd.Parameters.Clear();
        //db.sqlCmd.Parameters.Add("@giatrithuong", SqlDbType.Float).Value = giatrithuong;

        public void ClearParameters()
        {
            sqlCmd.Parameters.Clear(); 
        }
        public void  AddParameters(String key, SqlDbType type, Object value )
        {
            sqlCmd.Parameters.Add(key, type).Value = value;
            
        }

        public int getFirstIntValueSqlCatchException(string query,int exceptionVal)
        {
            try
            {
                sqlCmd.CommandText = query;
                return int.Parse(sqlCmd.ExecuteScalar().ToString());
            }
            catch (Exception e) { return exceptionVal; }
        }

        public int getFirstIntValueSqlCatchException(string query)
        {
            try
            {
                sqlCmd.CommandText = query;
                return int.Parse(sqlCmd.ExecuteScalar().ToString());
            }
            catch (Exception e) { return 0; }
        }
        public clsDB(string strConLocal)
        {
            this.pConnectionStr = strConLocal;
            m_Connection = new SqlConnection(pConnectionStr);
            if (m_Connection.State == ConnectionState.Closed)
                m_Connection.Open();
            sqlCmd = new SqlCommand("", m_Connection);
        }
        public string getFirstValueSql(string query)
        {
            sqlCmd.CommandText = query;
            return sqlCmd.ExecuteScalar().ToString();
        }

        public void BeginTran()
        {
            sqlTran = m_Connection.BeginTransaction(IsolationLevel.Snapshot);

            if (sqlCmd == null)
                sqlCmd = new SqlCommand("", m_Connection, sqlTran);
            else
                sqlCmd.Transaction = sqlTran;
        }

        public string pConnectionStr
        {
            set { this.connectionStr = value; }
            get
            {
                return connectionStr;
            }
        }


        public SqlConnection pConnection
        {
            set { this.m_Connection = value; }
            get
            {
                return m_Connection;
            }
        }

        public SqlTransaction pSqlTran
        {
            set { this.sqlTran = value; }
            get
            {
                return sqlTran;
            }
        }

        public SqlCommand pSqlCmd
        {
            set { this.sqlCmd = value; }
            get
            {
                return sqlCmd;
            }
        }


        public void CLose_Connection()
        {
            try
            {
                if (m_Connection != null)
                { 
                    if (sqlTran != null)
                    {
                        sqlTran.Dispose();
                        sqlTran = null;
                    }
                    sqlCmd.Dispose();
                    SqlConnection.ClearPool(m_Connection);
                    m_Connection.Dispose();
                    sqlCmd = null;
                    m_Connection = null;
                }
            }
            catch {}
        }

        public void CommitAndDispose()
        {
            try
            {
                if (  sqlTran != null )
                {
                    sqlTran.Commit();
                }
            }
            catch (Exception e) { }
            CLose_Connection();
        }

        public void RollbackAndDispose()
        {
            try
            {
                if (sqlTran != null)
                    sqlTran.Rollback();
            }
            catch (Exception e) { }
            CLose_Connection();
        }

        public  bool updateQuery(SqlCommand lenh, string query)
        {
            try
            {
                lenh.CommandText = query;
                lenh.ExecuteNonQuery();
                return true;
            }
            catch (Exception e) { return false; }
        }

        public bool updateQuery( string query)
        {
            try
            {
                sqlCmd.CommandText = query;
                sqlCmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e) { return false; }
        }

        
        public int updateQueryReturnInt(string query)
        {
            try
            {
                sqlCmd.CommandText = query;
                return sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e) { string a = e.Message; return -1; }
        }

        public int updateQueryReturnInt(string query, ref string msgError)
        {
            try
            {
                sqlCmd.CommandText = query;
                return sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e) { msgError = e.Message; return -1; }
        }

        public String getJsonObj(string query)
        {
            DataTable dt = new DataTable("Table");
            sqlCmd.CommandText = query;
            sqlCmd.CommandTimeout = 1200;
            //dt.Load(sqlCmd.ExecuteReader());
            //if (dt.Rows.Count >= 1)
            //    return XuLy.ParseDataRowToJson(dt.Rows[0]);
            //return null;

            var r = XuLy.Serialize(sqlCmd.ExecuteReader());
           
            string json = JsonConvert.SerializeObject(r, Formatting.Indented);
           
            return json;
        }
        public String getJsonArr(string query)
        {
            DataTable dt = new DataTable("Table");
            sqlCmd.CommandText = query;
            sqlCmd.CommandTimeout = 1200;
            dt.Load(sqlCmd.ExecuteReader());
         
            return XuLy.ParseDataTableToJSon(dt);

        }

        public DataTable getDataTable(string query)
        {
            DataTable dt = new DataTable("Table");
            sqlCmd.CommandText = query;
            sqlCmd.CommandTimeout = 1200;
            dt.Load(sqlCmd.ExecuteReader());
            return dt;
        }
        public static DataTable getDataTableStatic(string query)
        {
            DataTable dt = new DataTable("Table");
            SqlDataAdapter da = new SqlDataAdapter(query, clsDB.strConnSalesUp);
            da.Fill(dt);
            da.Dispose();
            return dt;
        }

        public string getFirstValueSql(SqlCommand lenh, string query)
        {
            lenh.CommandText = query;
            return lenh.ExecuteScalar().ToString();
        }

        public string getFirstStringValueSqlCatchException(SqlCommand lenh, string query)
        {
            try
            {
                lenh.CommandText = query;
                return lenh.ExecuteScalar().ToString();
            }
            catch (Exception e) { return ""; }
        }
        public string getFirstStringValueSqlCatchException(string query)
        {
            try
            {
                sqlCmd.CommandText = query;
                return sqlCmd.ExecuteScalar().ToString();
            }
            catch (Exception e) { return ""; }
        }

        public double getFirstDoubleValueSqlCatchException( string query)
        {
            try
            {
                sqlCmd.CommandText = query;
                return double.Parse(sqlCmd.ExecuteScalar().ToString());
            }
            catch (Exception e) { return 0; }
        }

        public int getFirsIntValueSqlCatchException(string query)
        {
            try
            {
                sqlCmd.CommandText = query;
                return int.Parse(sqlCmd.ExecuteScalar().ToString());
            }
            catch (Exception e) { return 0; }
        }

        public int getIntDoubleValueSqlCatchException(SqlCommand lenh, string query)
        {
            try
            {
                lenh.CommandText = query;
                return int.Parse(lenh.ExecuteScalar().ToString());
            }
            catch (Exception e) { return 0; }
        }

        /// <summary>
        /// ///////////// static Database function
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>

        public static  DataTable getDataTable(string connectionStrLocal,string query)
        {
            DataTable dt = new DataTable("Table");
            SqlDataAdapter da = new SqlDataAdapter(query, connectionStrLocal);
            da.Fill(dt);
            da.Dispose();
            return dt;
        }
        public static DataTable getDataTable(SqlDataAdapter da, string query)
        {
            DataTable dt = new DataTable("Table");
            da.SelectCommand.CommandText = query;
            da.SelectCommand.CommandTimeout = 120;
            da.Fill(dt);
            da.Dispose();
            return dt;
        }
        public static string getFirstStringValueSql(string connectionStrLocal, string query)
        {
            DataTable dt = new DataTable("Table");
            SqlDataAdapter da = new SqlDataAdapter(query, connectionStrLocal);
            da.Fill(dt);
            da.Dispose();
            string value = "";
            if (dt.Rows.Count > 0)
                value = dt.Rows[0][0].ToString();
            dt.Dispose();
            return value;
        }
        public static double getFirstDoubleValueSql( string query)
        {
            try
            {
                DataTable dt = new DataTable("Table");
                SqlDataAdapter da = new SqlDataAdapter(query, strConnSalesUp);
                da.Fill(dt);
                da.Dispose();
                string value = "0";
                if (dt.Rows.Count > 0)
                    value = dt.Rows[0][0].ToString();
                dt.Dispose();
                return double.Parse(value);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static int getFirstIntValueSql(string connectionStrLocal, string query)
        {
            try
            {
                DataTable dt = new DataTable("Table");
                SqlDataAdapter da = new SqlDataAdapter(query, connectionStrLocal);
                da.Fill(dt);
                da.Dispose();
                string value = "0";
                if (dt.Rows.Count > 0)
                    value = dt.Rows[0][0].ToString();
                dt.Dispose();
                return int.Parse(value);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this.RollbackAndDispose();
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~clsDB()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        public T ConvertObject<T>(DataTable dt)
        {
            T obj = Activator.CreateInstance<T>();
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                obj = GetItem<T>(row);

            }
            return obj;

        }
        public List<T> getListObject<T>(string query)
        {
            List<T> data = new List<T>();
            try
            {
                DataTable dt = this.getDataTable(query);
                T obj = Activator.CreateInstance<T>();

                foreach (DataRow row in dt.Rows)
                {
                    obj = GetItem<T>(row);
                    data.Add(obj);
                }

            }
            catch (Exception er)
            {

            }
            return data;

        }



        public T getModle<T>(string query)
        {


            DataTable dt = new DataTable("Table");
            sqlCmd.CommandText = query;
            sqlCmd.CommandTimeout = 120;
            dt.Load(sqlCmd.ExecuteReader());

            T obj = Activator.CreateInstance<T>();
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                obj = GetItem<T>(row);

            }
            return obj;

        }

        public List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (System.Reflection.PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

    }
}
