namespace System.Data.Extensions
{
    public interface IDbConnection
    {
        string Name { get; }

        string ConnectionString { get; }
    }
}
