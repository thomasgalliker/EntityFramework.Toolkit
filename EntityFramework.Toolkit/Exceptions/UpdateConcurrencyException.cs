using System;

namespace EntityFramework.Toolkit.Exceptions
{
    public class UpdateConcurrencyException : Exception
    {
        public UpdateConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
