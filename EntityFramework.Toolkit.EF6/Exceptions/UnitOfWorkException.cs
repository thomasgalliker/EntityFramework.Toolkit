using System;

namespace EntityFramework.Toolkit.EF6.Exceptions
{
    public class UnitOfWorkException : Exception
    {
        public UnitOfWorkException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
