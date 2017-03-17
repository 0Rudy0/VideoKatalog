using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;

namespace Video_katalog {
    public static class ComposeConnectionString {
        public static string Compose () {
            string pcName = System.Environment.MachineName;
            string databaseName = "VideoDatabase";
            string serverName = "SQLEXPRESS";
            //string dataSource = System.IO.Directory.GetCurrentDirectory () + "VideoDatabase2.sdf";
            //string connectionString = "Data Source=" + dataSource +";Persist Security Info=False";
            string connectionString = string.Format ("Data Source={0}\\{1};Initial Catalog={2};Integrated Security=True", pcName, serverName, databaseName);
            return connectionString;
        }
        public static string ComposeInitial() {
            //string pcName = System.Environment.MachineName;
            //string serverName = "SQLEXPRESS";
            //string connectionString = string.Format("Data Source={0}\\{1};Initial Catalog=master;Integrated Security=True", pcName, serverName);
            return System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerLocal"].ConnectionString;
            //return "Data Source=.\\SQLEXPRESS;Initial Catalog=VideoDatabaseFull;Integrated Security=True";
        }
        public static string ComposeCEString () {
            //string dataSource = System.IO.Directory.GetCurrentDirectory () + "\\VideoDatabase.sdf";
            //string dataSource = "C:\\Databases\\VideoDatabase.sdf";
            string dataSource = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments) + "\\Video Katalog\\VideoDatabase.sdf";
            string connectionString = "Data Source=" + dataSource + ";Persist Security Info=False;";
            //string connectionString = "Data Source=" + HardcodedStrings.appDataFolder + "VideoDatabase.sdf;Persist Security Info=False;";
            //SqlCeEngine engine = new SqlCeEngine(connectionString);
            //engine.Upgrade();
            return connectionString;
        }
        public static string ComposeMySqlRemoteCS() {
            string connectionStringFree = "server=db4free.net;" +
                "database=videokatalog;" +
                "uid=nabavahdd;" +
                "password=rudx1234;";

            string connectionString = "server=64.120.203.67;" +
                "database=hdfilmov_katalog;" +
                "uid=hdfilmov_user;" +
                "password=rudXYZ1%;";
            string connectionStringFreeMySQL = "server=SQL09.FREEMYSQL.NET;" +
                "database=videokatalog;" +
                "uid=nabavahdd;" +
                "password=rudx1234;";

            return connectionStringFreeMySQL;
        }
        public static string ComposeMySqlLocalCS () {
            string connectionString = "server=localhost;" +
                "database=videokatalog;" +
                "uid=root;" +
                "password=;";
            //string connectionString = "SERVER=localhost;" +
            //    "DATABASE=videokatalog;" +
            //    "UID=root;";
            return connectionString;
        }
        public static string ComposeSqlRemote() {
            return System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerRemote"].ConnectionString;
        }
    }
}
