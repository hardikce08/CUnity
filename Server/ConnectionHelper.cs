using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.EntityClient;
using System.Linq;
using System.Threading.Tasks;

namespace CUnity.Server
{
    public class ConnectionHelper:ControllerBase
    {
        public string DbConnectionStringKey
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultConnection"];
            }
        }
        public string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[DbConnectionStringKey].ConnectionString;
            }
        }
        static string _entityConnection = null;
        public string EntityConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_entityConnection))
                {
                    EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
                    entityBuilder.Provider = "System.Data.SqlClient";
                    entityBuilder.ProviderConnectionString = ConnectionString;
                    entityBuilder.Metadata = @"res://*/OnBoarding.csdl|res://*/OnBoarding.ssdl|res://*/OnBoarding.msl";
                    _entityConnection = entityBuilder.ToString();
                }
                return _entityConnection;
            }
        }
    }
}
