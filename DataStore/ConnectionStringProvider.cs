using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MyBedrockTest.DataStore
{
    internal class ConnectionStringProvider
    {
        public static string GetDBConnectionString()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            string connectionString= config.GetValue<string>("ConnectionStrings:KBStoreDB");
            return connectionString;
        }

    }
}
