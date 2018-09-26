using System;

namespace EntityFramework.Toolkit.EF6.Exceptions
{
    public class UpdateConcurrencyException : Exception
    {
        public UpdateConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
