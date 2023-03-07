using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Data;

namespace DataTester.repository;
public class TestRepository: BaseRepository
{

    private const string SQL_GET_ACCOUNT_USERS = "Fleet.dbo.spGetUsers";
    

    public TestRepository(IConfiguration configuration, ILogger<TestRepository> logger) : base(configuration, logger)
    {
    }

}
