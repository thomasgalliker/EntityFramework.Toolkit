using System;
using System.Data.SqlClient;

namespace EntityFramework.Toolkit.Testing
{
    public static class ConnectionStringGenerator
    {
        /// <summary>
        /// Adds a random number to the given <param name="connectionString">connectionString</param> parameter.
        /// </summary>
        public static string RandomizeDatabaseName(this string connectionString, int randomTokenLength = 5)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            var randomToken = GetRandomToken(randomTokenLength);
            if (!string.IsNullOrEmpty(connectionStringBuilder.InitialCatalog))
            {
                connectionStringBuilder.InitialCatalog += randomToken;
            }
            else if (!string.IsNullOrEmpty(connectionStringBuilder.AttachDBFilename))
            {
                var dbFileExtension = ".mdf";
                var randomAttachDbFilename = connectionStringBuilder.AttachDBFilename
                    .Replace(dbFileExtension, "_" + randomToken + dbFileExtension);

                connectionStringBuilder.AttachDBFilename = randomAttachDbFilename;
                connectionStringBuilder.InitialCatalog = randomAttachDbFilename
                    .Replace("|DataDirectory|\\", "");
            }

            string randomizedConnectionString = connectionStringBuilder.ToString();
            return randomizedConnectionString;
        }

        /// <summary>
        /// Generates a random upper-invariant string of <paramref name="randomTokenLength"/>.
        /// </summary>
        /// <param name="randomTokenLength"></param>
        /// <returns></returns>
        private static string GetRandomToken(int randomTokenLength)
        {
            string randomString = Guid.NewGuid().ToString()
                .Replace("-", "")
                .Substring(0, randomTokenLength)
                .ToUpperInvariant();

            return randomString;
        }
    }
}
