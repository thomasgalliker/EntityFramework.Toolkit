using System;

namespace EntityFramework.Toolkit.Exceptions
{
    public class UnitOfWorkException : Exception
    {
        public UnitOfWorkException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
