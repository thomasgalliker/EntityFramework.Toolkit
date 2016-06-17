namespace System.Data.Extensions.Exceptions
{
    public class UnitOfWorkException : Exception
    {
        public UnitOfWorkException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
