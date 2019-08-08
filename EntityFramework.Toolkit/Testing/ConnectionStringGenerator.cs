using System;
using System.Data.SqlClient;

namespace EntityFramework.Toolkit.Testing
{
    public static class ConnectionStringGenerator
    {
        /// <summary>
        /// Adds a random number to the given <param name="connectionString">connectionString</param> parameter.
        /// </summary>
        /// <param name="connectionString">The original connection string.</param>
        /// <param name="randomTokenLength">The length of the generated random number.</param>
        /// <param name="prefix">A prefix attach between database name and random token.</param>
        /// <returns></returns>
        public static string RandomizeDatabaseName(this string connectionString, int randomTokenLength = 5, string prefix = "_")
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            var randomToken = GetRandomToken(randomTokenLength);
            if (!string.IsNullOrEmpty(connectionStringBuilder.InitialCatalog))
            {
                connectionStringBuilder.InitialCatalog += prefix + randomToken;
            }
            else if (!string.IsNullOrEmpty(connectionStringBuilder.AttachDBFilename))
            {
                var dbFileExtension = ".mdf";
                var randomAttachDbFilename = connectionStringBuilder.AttachDBFilename
                    .Replace(dbFileExtension, prefix + randomToken + dbFileExtension);

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
            if (randomTokenLength > 32)
            {
                throw new ArgumentException($"{nameof(randomTokenLength)} must not be greater than 32", nameof(randomTokenLength));
            }

            string randomString = Guid.NewGuid().ToString()
                .Replace("-", "")
                .Substring(0, randomTokenLength)
                .ToUpperInvariant();

            return randomString;
        }
    }
}
