using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Data;
using Toolkit.Serialization;
using Toolkit.Extensions;
using System.Linq;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Diagnostics;
using System.Threading.Tasks;
using Toolkit.Model;

namespace Toolkit.Data
{
    public class BaseRepository
    {
        private string _connectionString;
        private string _sqliteConnectionString;
        private int _timeout;
        IConfiguration _configuration = null;
        public ILogger _logger;
        public BaseRepository(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        protected string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(_connectionString)) return _connectionString;
                _connectionString = _configuration.GetConnectionString("DBConnection") ?? "";
                return _connectionString;
            }
        }
        protected string SqliteConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(_sqliteConnectionString)) return _sqliteConnectionString;
                _sqliteConnectionString = _configuration.GetConnectionString("SqliteConnection") ?? "";
                return _sqliteConnectionString;
            }
        }

        
        

        protected int Timeout
        {
            get
            {
                if (_timeout != default(int)) return _timeout;
                _timeout = _configuration.GetSection("AppSettings")["DB_TIMEOUT"].StringToInt();
                return _timeout;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandType"></param>
        /// <param name="sql"></param>
        /// <param name="commandParams"></param>
        /// <returns></returns>
        public List<T> GetTable<T>(CommandType commandType, string sql, SqlParameterList commandParams) where T : class
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams.Params);
            List<T> result = new List<T>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {

                using (SqlDataReader dr = SqlHelper.ExecuteReader(conn, commandType, sql, commandParams.ToArray()))
                {
                    while (dr.Read())
                    {
                        result.Add(Serializer.DeSerialize<T>(dr));
                    }
                }

            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            _logger.LogTrace("After Execute {@sql}, time = {@elapsed}", sql, elapsed);
            return result;
        }

        public int ExecuteNonQuery(CommandType commandType, string sql, SqlParameterList commandParams)
        {
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            int result = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var conn = new SqlConnection(ConnectionString))
            {
                var qResult = SqlHelper.ExecuteNonQuery(conn, commandType, sql, commandParams.ToArray());//  conn.Execute(sql, commandParams, commandType: commandType, commandTimeout: SqlHelper.TIMEOUT);
                result = qResult;
            }
            stopwatch.Stop();
            _logger.LogTrace($"After Execute {sql}, time = {stopwatch.Elapsed}");
            return result;
        }

        public T GetTableItem<T>(CommandType commandType, string sql, SqlParameterList commandParams) where T : class
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            _logger.LogTrace("Before Execute {@sql} ", sql);
            List<T> result = new List<T>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {

                using (SqlDataReader dr = SqlHelper.ExecuteReader(conn, commandType, sql, commandParams.ToArray()))
                {
                    while (dr.Read())
                    {
                        result.Add(Serializer.DeSerialize<T>(dr));
                    }
                }

            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            _logger.LogTrace("After Execute {@sql}, time = {@elapsed}", sql, elapsed);
            return result.FirstOrDefault();
        }


        public List<T> GetTableDapper<T>(CommandType commandType, string sql, object commandParams) where T : class
        {
            return GetTableDapper<T>(ConnectionString, commandType, sql, commandParams);
        }

        public async Task<List<T>> GetTableDapperAsync<T>(CommandType commandType, string sql, object commandParams) where T : class
        {
            return await GetTableDapperAsync<T>(ConnectionString, commandType, sql, commandParams);
        }

        public List<T> GetTableDapper<T>(string connectionString, CommandType commandType, string sql, object commandParams) where T : class
        {
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            _logger.LogTrace("Before Execute {@sql} ", sql);
            List<T> result = new List<T>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var conn = new SqlConnection(connectionString))
            {
                var qResult = conn.Query<T>(sql, commandParams, commandType: commandType, commandTimeout: SqlHelper.TIMEOUT);
                result = qResult.ToList();
            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            _logger.LogTrace("After Execute {@sql}, time = {@elapsed}", sql, elapsed);
            return result;
        }

        public async Task<List<T>> GetTableDapperAsync<T>(string connectionString, CommandType commandType, string sql, object commandParams) where T : class
        {
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            _logger.LogTrace("Before Execute {@sql} ", sql);
            List<T> result = new List<T>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var conn = new SqlConnection(connectionString))
            {
                var qResult = await conn.QueryAsync<T>(sql, commandParams, commandType: commandType, commandTimeout: SqlHelper.TIMEOUT);
                result = qResult.ToList();
            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            _logger.LogTrace("After Execute {@sql}, time = {@elapsed}", sql, elapsed);
            return result;
        }

        public List<T> GetTableDapper<T>(CommandType commandType, string sql, object commandParams, out object outParam) where T : class
        {
            outParam = null;
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            List<T> result = new List<T>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var conn = new SqlConnection(ConnectionString))
            {
                var qResult = conn.QueryMultiple(sql, commandParams, commandType: commandType, commandTimeout: SqlHelper.TIMEOUT);

                result = qResult.Read<T>().ToList();
                outParam = qResult.Read();
            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            _logger.LogTrace("After Execute {@sql}, time = {@elapsed}", sql, elapsed);
            return result;
        }
        public int ExecuteDapper(CommandType commandType, string sql, object commandParams, int? userId = null)
        {
            return ExecuteDapper(ConnectionString, commandType, sql, commandParams, SqlHelper.TIMEOUT, userId);
        }

        public int ExecuteDapper(CommandType commandType, string sql, object commandParams, int commandTimeout, int? userId = null)
        {
            return ExecuteDapper(ConnectionString, commandType, sql, commandParams, commandTimeout, userId);
        }

        public async Task<int> ExecuteDapperAsync(CommandType commandType, string sql, object commandParams, int? userId = null)
        {
            return await ExecuteDapperAsync(ConnectionString, commandType, sql, commandParams, SqlHelper.TIMEOUT, userId);
        }

        public int ExecuteDapper(string connectionString, CommandType commandType, string sql, object commandParams, int commandTimeout, int? userId = null)
        {
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            int result = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //var con = $"{connectionString};Application Name=FleetCore.api;"; // _{userId}
            using (var conn = new SqlConnection(ConnectionString))
            {

                if (userId.HasValue)
                {
                    conn.Open();
                    var qur = $"EXEC sys.sp_set_session_context @key = N'UserId', @value = {userId};";// $"DECLARE @BinVar binary(128) SET @BinVar = CAST( {userId} as binary(128) ) SET CONTEXT_INFO @BinVar";
                    var rr = conn.Execute(qur, commandType: CommandType.Text);
                }
                var qResult = conn.Execute(sql, commandParams, commandType: commandType, commandTimeout: commandTimeout);
                result = qResult;
            }
            stopwatch.Stop();
            _logger.LogTrace($"After Execute {sql}, time = {stopwatch.Elapsed}");
            return result;
        }

        public async Task<int> ExecuteDapperAsync(string connectionString, CommandType commandType, string sql, object commandParams, int commandTimeout, int? userId = null)
        {
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            int result = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var conn = new SqlConnection(connectionString))
            {
                if (userId.HasValue)
                {
                    conn.Open();
                    var qur = $"EXEC sys.sp_set_session_context @key = N'UserId', @value = {userId.Value};";
                    var rr = await conn.ExecuteAsync(qur, commandType: CommandType.Text);
                }

                var qResult = await conn.ExecuteAsync(sql, commandParams, commandType: commandType, commandTimeout: commandTimeout);
                result = qResult;
            }
            stopwatch.Stop();
            _logger.LogTrace($"After Execute {sql}, time = {stopwatch.Elapsed}");
            return result;
        }

        public T GetTableItemDapper<T>(CommandType commandType, string sql, object commandParams) where T : class
        {
            return GetTableItemDapper<T>(ConnectionString, commandType, sql, commandParams);
        }

        public async Task<T> GetTableItemDapperAsync<T>(CommandType commandType, string sql, object commandParams) where T : class
        {
            return await GetTableItemDapperAsync<T>(ConnectionString, commandType, sql, commandParams);
        }

        public T GetTableItemDapper<T>(string connectionString, CommandType commandType, string sql, object commandParams) where T : class
        {
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            T result;
            using (var conn = new SqlConnection(connectionString))
            {
                //  var xx =  conn.QueryFirstOrDefault(sql, commandParams, commandType: commandType, commandTimeout: SqlHelper.TIMEOUT);
                var qResult = conn.QueryFirstOrDefault<T>(sql, commandParams, commandType: commandType, commandTimeout: SqlHelper.TIMEOUT);
                result = qResult;
            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            _logger.LogTrace("After Execute {@sql}, time = {@elapsed}", sql, elapsed);
            return result;
        }

        public async Task<T> GetTableItemDapperAsync<T>(string connectionString, CommandType commandType, string sql, object commandParams) where T : class
        {
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            T result;
            using (var conn = new SqlConnection(connectionString))
            {
                var qResult = await conn.QueryFirstOrDefaultAsync<T>(sql, commandParams, commandType: commandType, commandTimeout: SqlHelper.TIMEOUT);
                result = qResult;
            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            _logger.LogTrace("After Execute {@sql}, time = {@elapsed}", sql, elapsed);
            return result;
        }

        public T GetMultiTableItemDapper<T, T2>(CommandType commandType, string sql, object commandParams, out List<T2> outParam) where T : class where T2 : class
        {

            return GetMultiTableItemDapper<T, T2>(ConnectionString, commandType, sql, commandParams, out outParam);
        }
        public T GetMultiTableItemDapper<T, T2>(string connectionString, CommandType commandType, string sql, object commandParams, out List<T2> outParam) where T : class where T2 : class
        {
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            T result;
            using (var conn = new SqlConnection(ConnectionString))
            {
                var qResult = conn.QueryMultiple(sql, commandParams, commandType: commandType, commandTimeout: SqlHelper.TIMEOUT);

                result = qResult.ReadFirstOrDefault<T>();
                outParam = qResult.IsConsumed ? new List<T2>() : qResult.Read<T2>().ToList();
            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            _logger.LogTrace("After Execute {@sql}, time = {@elapsed}", sql, elapsed);
            return result;
        }

        public string GetTableItemValue(CommandType commandType, string sql, object commandParams)
        {
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string result;
            using (var conn = new SqlConnection(ConnectionString))
            {
                var qResult = conn.QueryFirstOrDefault<string>(sql, commandParams, commandType: commandType, commandTimeout: SqlHelper.TIMEOUT);
                result = qResult;
            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            _logger.LogTrace("After Execute {@sql}, time = {@elapsed}", sql, elapsed);
            return result;
        }
        public List<T> GetTable<T>(string sql, object commandParams) where T : class
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            List<T> result = new List<T>();
            using (var conn = new SqliteConnection(SqliteConnectionString))
            {
                var qResult = conn.Query<T>(sql, commandParams, commandTimeout: SqlHelper.TIMEOUT);
                result = qResult.ToList();
            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            _logger.LogTrace("After Execute {@sql}, time = {@elapsed}", sql, elapsed);
            return result;
        }
        public T GetTableItem<T>(string sql, object commandParams) where T : class
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _logger.LogTrace($"Before Execute {sql}");
            T result;
            using (var conn = new SqliteConnection(SqliteConnectionString))
            {
                var qResult = conn.QueryFirstOrDefault<T>(sql, commandParams, commandTimeout: SqlHelper.TIMEOUT);
                result = qResult;
            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            _logger.LogTrace("After Execute {@sql}, time = {@elapsed}", sql, elapsed);
            return result;
        }

        public int ExecuteDapper(string sql, object commandParams)
        {
            _logger.LogTrace("Before Execute {@sql} {@commandParams}", sql, commandParams);
            int result = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var conn = new SqliteConnection(SqliteConnectionString))
            {
                var qResult = conn.Execute(sql, commandParams, commandTimeout: SqlHelper.TIMEOUT);
                result = qResult;
            }
            stopwatch.Stop();
            _logger.LogTrace($"After Execute {sql}, time = {stopwatch.Elapsed}");
            return result;
        }

        public void LogError(string errorMsg, string spName, object request)
        {
            string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
            _logger.LogError(errorMsg, new { sp = spName, m = methodName, req = request });
        }

        public void LogError(Exception ex, string errorMsg, string spName, object request)
        {
            string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
            _logger.LogError(ex, errorMsg, new { sp = spName, m = methodName, req = request });
        }

        public BaseErrorInfo GetError(long errorCode)
        {
            string cmdText = "PAI..spGetError";
            var p = new DynamicParameters();
            p.Add("Code", errorCode);

            BaseErrorInfo err;
            err = GetTableItemDapper<BaseErrorInfo>(CommandType.StoredProcedure, cmdText, p);
            if (err == null)
            {
                err = new BaseErrorInfo() { Code = errorCode };
            }
            return err;
        }
    }
}
