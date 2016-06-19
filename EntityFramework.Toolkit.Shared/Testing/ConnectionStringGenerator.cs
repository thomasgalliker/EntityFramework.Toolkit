using System;

namespace EntityFramework.Toolkit.Testing
{
    public static class ConnectionStringGenerator
    {
        /// <summary>
        /// Adds a random number to the given <param name="connectionString">connectionString</param> parameter.
        /// </summary>
        public static string RandomizeDatabaseName(this string connectionString)
        {
            if (!connectionString.Contains("{0}"))
            {
                throw new InvalidOperationException("ConnectionString does not contain a placeholder for randomization!");
            }

            string randomString = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 5);
            string randomizedConnectionString = string.Format(connectionString, randomString);
            return randomizedConnectionString;
        }
    }

}
