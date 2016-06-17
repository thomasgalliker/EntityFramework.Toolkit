namespace System.Data.Extensions
{
    public interface IDbConnection
    {
        string Name { get; }

        string ConnectionString { get; }

        bool LazyLoadingEnabled { get; set; }

        bool ProxyCreationEnabled { get; set; }
    }
}
