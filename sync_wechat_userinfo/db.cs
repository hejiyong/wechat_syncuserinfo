using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace sync_wechat_userinfo
{
    public static class db
    {
        public static IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("App.json", false, true)
            .Build();

        public static IFreeSql fsql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString((FreeSql.DataType)Enum.Parse(typeof(FreeSql.DataType), config["db:dbType"]), config["db:connectionString"])
            .UseAutoSyncStructure(true) //自动同步实体结构到数据库
            .Build();
    }
}
