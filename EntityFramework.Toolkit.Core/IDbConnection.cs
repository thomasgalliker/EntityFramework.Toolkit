namespace EntityFramework.Toolkit.Core
{
    public interface IDbConnection
    {
        string Name { get; }

        string ConnectionString { get; }

        bool LazyLoadingEnabled { get; set; }

        bool ProxyCreationEnabled { get; set; }

        bool ForceInitialize { get; }
    }
}
