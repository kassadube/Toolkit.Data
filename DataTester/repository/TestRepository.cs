using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Data;
using Toolkit.Model;

namespace DataTester.repository;
public class TestRepository: BaseRepository
{

    private const string SQL_GET_ACCOUNT_USERS = "Fleet.dbo.spGetUsers";
    private const string SQL_GET_ACCOUNT_DETAILS = "Select * from AccountDetails where Id = @AccountId";


    public TestRepository(IConfiguration configuration, ILogger<TestRepository> logger) : base(configuration, logger)
    {
    }
    public BaseResultInfo GetAccountUsers(BaseREQ req)
    {
        BaseResultInfo res = new BaseResultInfo();
        try
        {
            res.ResultObject = base.GetTableDapper<AccountUserInfo>(CommandType.StoredProcedure, SQL_GET_ACCOUNT_USERS, new { AccountId = req.AccountId });
        }
        catch (Exception ex)
        {
            base._logger.LogError(ex, "Data {data}", new { sp = SQL_GET_ACCOUNT_USERS, m = "GetAccountUsers", req = req });
            res.Error = new BaseErrorInfo() { Message = ex.Message };
        }
        return res;
    }
    public  BaseResultInfo GetAccountDetails(BaseREQ req)
    {
        DataResultInfo<AccountInfo> result = new DataResultInfo<AccountInfo>();
        result.ResultObject = base.GetTableItem<AccountInfo>(SQL_GET_ACCOUNT_DETAILS, new { AccountId = req.AccountId });
        return result;
    }


}
