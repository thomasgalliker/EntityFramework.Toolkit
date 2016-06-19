using System.Data.Entity.Validation;
using System.Text;

namespace EntityFramework.Toolkit.Extensions
{
    internal static class DbEntityValidationExceptionFormatter
    {
        internal static string GetFormattedErrorMessage(this DbEntityValidationException validationException)
        {
            var stringBuilder = new StringBuilder();

            // In case something goes wrong during entity validation
            // we trace the affected properties with its problems to the console and rethrow the exception
            foreach (var result in validationException.EntityValidationErrors)
            {
                stringBuilder.AppendLine(
                    string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        result.Entry.Entity.GetType().Name,
                        result.Entry.State));

                foreach (var ve in result.ValidationErrors)
                {
                    stringBuilder.AppendLine(string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                        ve.PropertyName,
                        result.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                        ve.ErrorMessage));
                }
                stringBuilder.AppendLine();
            }

            string errorMessage = stringBuilder.ToString();
            return errorMessage;
        }
    }
}