using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;

namespace EntityFramework.Toolkit.Extensions
{
    internal static class DbUpdateExceptionFormatter
    {
        private static readonly int SqlStatementTerminationNumber = 3621;

        internal static string GetFormattedErrorMessage(this DbUpdateException dbUpdateException)
        {
            var stringBuilder = new StringBuilder();

            var sqlException = dbUpdateException.InnerException?.InnerException as SqlException;

            foreach (var entry in dbUpdateException.Entries)
            {
                stringBuilder.AppendLine(
                    $"Entity of type \"{entry.Entity.GetType().GetFormattedName()}\" in state \"{entry.State}\" has caused a DbUpdateException.");

                if (entry.State == EntityState.Modified)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("Modified properties:");
                    foreach (var property in entry.GetProperties())
                    {
                        stringBuilder.AppendLine(
                            $"- ({(property.IsModified ? "X" : " ")}) {property.Name}: OriginalValue =\"{property.OriginalValue}\", CurrentValue=\"{property.CurrentValue}\"");
                    }
                }

                if (sqlException != null)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("SqlException Errors:");

                    foreach (SqlError sqlExceptionError in sqlException.Errors)
                    {
                        if (sqlExceptionError.Number == SqlStatementTerminationNumber)
                        {
                            continue;
                        }

                        stringBuilder.AppendLine($"-> Number  {sqlExceptionError.Number}: {sqlExceptionError.Message}");

                        switch (sqlExceptionError.Number)
                        {
                            case 242:
                                stringBuilder.AppendLine("   Following properties may have cause the implications:");
                                foreach (string propertyName in entry.CurrentValues.PropertyNames)
                                {
                                    var propertyInfo = entry.Entity.GetType().GetProperty(propertyName);
                                    var propertyType = propertyInfo.PropertyType;
                                    DateTime? propertyValue = null;
                                    if (propertyType == typeof(DateTime?))
                                    {
                                        propertyValue = entry.CurrentValues.GetValue<DateTime?>(propertyName);
                                    }
                                    if (propertyType == typeof(DateTime))
                                    {
                                        propertyValue = entry.CurrentValues.GetValue<DateTime>(propertyName);
                                    }
                                    if (propertyValue.HasValue && !IsValidSqlServerDatetime(propertyValue.Value))
                                    {
                                        stringBuilder.AppendLine($"   - Property: \"{propertyName}\", Type: {propertyType.GetFormattedName()}, Value: \"{propertyValue.Value}\"");
                                    }
                                }
                                break;

                            case 547:
                                var relationshipSourceColumnName = GetSourceColumnName(sqlExceptionError.Message);
                                stringBuilder.AppendLine("   Following properties may have cause the implications:");
                                foreach (string propertyName in entry.CurrentValues.PropertyNames)
                                {
                                    var propertyInfo = entry.Entity.GetType().GetProperty(propertyName);
                                    var propertyType = propertyInfo.PropertyType;
                                    var propertyValue = entry.CurrentValues.GetValue<object>(propertyName);

                                    var isColumnPossiblyAffected = propertyName.Contains(relationshipSourceColumnName);

                                    stringBuilder.AppendLine(
                                        $"   - ({(isColumnPossiblyAffected ? "X" : " ")}) {propertyName}: Type: {propertyType.GetFormattedName()}, Value: \"{propertyValue}\"");
                                }

                                stringBuilder.AppendLine("   This operation failed because another data entry uses this entry.");
                                break;

                            case 2601:
                                stringBuilder.AppendLine("   One of the properties is marked as Unique index and there is already an entry with that value.");
                                break;
                        }
                    }
                }
            }

            string errorMessage = stringBuilder.ToString();
            return errorMessage;
        }

        private static string GetSourceColumnName(string sqlException)
        {
            string relationshipColumnName = null;
            var firstEscape = "\"";
            var firstQuoteIndex = sqlException.IndexOf(firstEscape, StringComparison.Ordinal);
            if (firstQuoteIndex >= 0)
            {
                var secondEscape = "\"";
                var secondQuoteIndex = sqlException.IndexOf(secondEscape, firstQuoteIndex + 1, StringComparison.Ordinal);
                if (secondQuoteIndex >= 0)
                {
                    var relationshipName = sqlException.Substring(firstQuoteIndex + 1, secondQuoteIndex - firstQuoteIndex - 1);
                    var relationshipTableAndColumn = relationshipName.Substring(relationshipName.LastIndexOf('.') + 1);
                    relationshipColumnName = relationshipTableAndColumn.Substring(relationshipTableAndColumn.IndexOf('_') + 1);
                }
            }

            return relationshipColumnName;
        }

        /// <summary>
        ///     Checks if <paramref name="dateTime" /> is between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.
        /// </summary>
        /// <remarks>
        ///     SQL Server defines two different datetime formats:
        ///     The datetime datatype is capable of storing dates in the range 1753-01-01 to 9999-12-31.
        ///     The datetime2 datatype was introduced in SQL Server 2008. The range of dates that it is capable of storing is
        ///     0001-01-01 to 9999-12-31.
        /// </remarks>
        private static bool IsValidSqlServerDatetime(this DateTime dateTime)
        {
            if (dateTime < SqlDateTime.MinValue.Value || dateTime > SqlDateTime.MaxValue.Value)
            {
                return false;
            }

            return true;
        }
    }
}