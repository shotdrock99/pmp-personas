using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using System.IO;
using ModernizacionPersonas.Common.Configuration;

namespace ModernizacionPersonas.DAL
{
    public class ConnectionFactory
    {
        private static AppConfiguration appConfig = new AppConfiguration();        

        public static SqlConnection Default()
        {
            //var strConn = "Data Source=solidod;Initial Catalog=Portales_desarrollo;Integrated Security=True";
            var connection = new SqlConnection(appConfig.ConnectionString);
            connection.Open();
            return connection;
        }

        public static SqlConnection SISEConnection()
        {
            //var strConn = "Data Source=solidod;Initial Catalog=Portales_desarrollo;Integrated Security=True";
            var connection = new SqlConnection(appConfig.SISEConnectionString);
            connection.Open();
            return connection;
        }

        public static SqlConnection SoligesproConnection()
        {
            //var strConn = "Data Source=solidod;Initial Catalog=Portales_desarrollo;Integrated Security=True";
            var connection = new SqlConnection(appConfig.SoligesproConnectionString);
            connection.Open();
            return connection;
        }

        public static SqlConnection ReportesWebConnection()
        {
            //var strConn = "Data Source=solidod;Initial Catalog=Portales_desarrollo;Integrated Security=True";
            var connection = new SqlConnection(appConfig.ReportesWebConnectionString);
            connection.Open();
            return connection;
        }
    }
}
