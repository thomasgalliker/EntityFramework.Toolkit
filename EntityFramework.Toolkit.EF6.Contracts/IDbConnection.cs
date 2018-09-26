namespace EntityFramework.Toolkit.EF6.Contracts
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
