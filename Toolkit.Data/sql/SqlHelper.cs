using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.Threading.Tasks;

namespace Toolkit.Data
{
    public abstract class SqlHelper
    {


        // Hashtable to store cached parameters
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        public static int TIMEOUT
        {
            get;
            set;
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            var StartTime = DateTime.Now;
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters,null);
                int val;
                try
                {
                     val = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    
                    throw ex;
                }
                cmd.Parameters.Clear();               
                return val;
            }
        }

        public static int ExecuteNonQuery(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return trans == null ? ExecuteNonQuery(conn, cmdType, cmdText, commandParameters) : ExecuteNonQuery(trans, cmdType, cmdText, commandParameters);
        }
        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">an existing database connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            var StartTime = DateTime.Now;
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters,null);
            int val;
            try
            {
                val = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
            cmd.Parameters.Clear();            
            return val;
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) using an existing SQL Transaction 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">an existing sql transaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            var StartTime = DateTime.Now;
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters,null);
            int val = 0;
            try
            {
                val = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                
                throw ex;
            }
           
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute a SqlCommand that returns a resultset against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>A SqlDataReader containing the results</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                // we use a try/catch here because if the method throws an exception we want to 
                // close the connection throw code, because no datareader will exist, hence the 
                // commandBehaviour.CloseConnection will not work
                try
                {
                    return ExecuteReader(conn, cmdType, cmdText, commandParameters);
                }
                catch(Exception ex)
                {
                    
                    throw ex;
                }
            }
        }
        
        public static SqlDataReader ExecuteReader(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return trans == null ? ExecuteReader(conn, cmdType, cmdText, commandParameters) : ExecuteReader(trans, cmdType, cmdText, commandParameters);
        }
        
        public static SqlDataReader ExecuteReader(SqlConnection conn, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(conn, cmdType, cmdText,null, commandParameters);
        }

        public static async Task<SqlDataReader> ExecuteReaderAsync(SqlConnection conn, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return await ExecuteReaderAsync(conn, cmdType, cmdText, null, commandParameters);
        }

        public static SqlDataReader ExecuteReader(SqlConnection conn, CommandType cmdType, string cmdText,int? timeout, params SqlParameter[] commandParameters)
        {
            var StartTime = DateTime.Now;
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters, null);
            SqlDataReader rdr;
            try
            {
                 rdr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            cmd.Parameters.Clear();          
            return rdr;
        }

        public static async Task<SqlDataReader> ExecuteReaderAsync(SqlConnection conn, CommandType cmdType, string cmdText, int? timeout, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters, null);
            SqlDataReader rdr;
            try
            {
                rdr = await cmd.ExecuteReaderAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            cmd.Parameters.Clear();
            return rdr;
        }

        public static SqlDataReader ExecuteReader(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            var StartTime = DateTime.Now;
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters,null);
            SqlDataReader rdr;
            try
            {
                rdr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
            cmd.Parameters.Clear();
           
            return rdr;

        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            var StartTime = DateTime.Now;
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters,null);
                object val;
                try
                {
                    val = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    
                    throw ex;
                }
                cmd.Parameters.Clear();
                
                return val;
            }
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">an existing database connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            var StartTime = DateTime.Now;
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters,null);
            object val;
            try
            {
                val = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
            cmd.Parameters.Clear();
            
            return val;
        }

        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            var StartTime = DateTime.Now;
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters,null);
            XmlReader val ;
            try
            {
                val = cmd.ExecuteXmlReader();
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
            cmd.Parameters.Clear();
           
            return val;
        }

        public static string ExecuteStringXmlReader(SqlConnection connection,SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            return trans == null ? ExecuteStringXmlReader(trans, cmdType, cmdText, commandParameters) : ExecuteStringXmlReader(connection, cmdType, cmdText, commandParameters);
            
        }
        
        public static string ExecuteStringXmlReader(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            var StartTime = DateTime.Now;
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters,null);
            XmlReader val;
            try
            {
                val = cmd.ExecuteXmlReader();
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
            cmd.Parameters.Clear();
            StringBuilder res = new StringBuilder("");
            val.Read();
            while (val.ReadState != ReadState.EndOfFile)
            {
                res.Append(val.ReadOuterXml());
            }

            val.Close();
           
            return res.ToString();
        }
        
        public static string ExecuteStringXmlReader(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            var StartTime = DateTime.Now;
            SqlCommand cmd = new SqlCommand();
            XmlReader val;
            try
            {
                val = cmd.ExecuteXmlReader();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            cmd.Parameters.Clear();
            string res = "";
            val.Read();
            while (val.ReadState != ReadState.EndOfFile)
            {
                res += val.ReadOuterXml();
            }

            val.Close();
            
            return res;
        }

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParms">an array of SqlParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached SqlParamters array</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="conn">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        public static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms, int? timeout)
        {

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandTimeout = TIMEOUT;
            if (trans != null)
            {                
                cmd.Transaction = trans;
            }

            cmd.CommandType = cmdType;
            StringBuilder paramsStr =new StringBuilder (cmdText);
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    paramsStr.AppendFormat(" {0} = {1},",parm.ParameterName,parm.Value);
                    cmd.Parameters.Add(parm);
                }
            }
        }

        public static async void PrepareCommandAsync(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms, int? timeout)
        {

            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }

            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandTimeout = TIMEOUT;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }

            cmd.CommandType = cmdType;
            StringBuilder paramsStr = new StringBuilder(cmdText);
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    paramsStr.AppendFormat(" {0} = {1},", parm.ParameterName, parm.Value);
                    cmd.Parameters.Add(parm);
                }
            }
        }
    }
}

