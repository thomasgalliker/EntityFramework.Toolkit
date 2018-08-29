using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace EntityFramework.Toolkit
{
    public static class DbConnectionExtensions
    {
        public static void DropDatabase(this IDbConnection connection)
        {
            var databaseName = GetDatabaseName(connection);
            var str = $@"USE master;
                         ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                         DROP DATABASE [{databaseName}];";

            try
            {
                using (DbCommand command = new SqlConnection(connection.ConnectionString).CreateCommand())
                {
                    command.CommandText = str;
                    command.CommandType = CommandType.Text;
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static string GetDatabaseName(this IDbConnection connection)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connection.ConnectionString);
            if (!string.IsNullOrEmpty(connectionStringBuilder.InitialCatalog))
            {
                return connectionStringBuilder.InitialCatalog;
            }

            if (!string.IsNullOrEmpty(connectionStringBuilder.AttachDBFilename))
            {
                return connectionStringBuilder.AttachDBFilename;
            }

            return null;
        }
    }
}