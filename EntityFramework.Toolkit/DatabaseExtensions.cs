using System.Data.Entity;

namespace System.Data.Extensions
{
    public static class DatabaseExtensions
    {
        public static void KillConnectionsToTheDatabase(this Database database)
        {
            var databaseName = database.Connection.Database;
            const string sqlFormat = @"
             USE master; 

             DECLARE @databaseName VARCHAR(50);
             SET @databaseName = '{0}';

             declare @kill varchar(8000) = '';
             select @kill=@kill+'kill '+convert(varchar(5),spid)+';'
             from master..sysprocesses 
             where dbid=db_id(@databaseName);

             exec (@kill);";

            var sql = string.Format(sqlFormat, databaseName);

            try
            {
                using (var command = database.Connection.CreateCommand())
                {
                    command.CommandText = sql;
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
    }
}

