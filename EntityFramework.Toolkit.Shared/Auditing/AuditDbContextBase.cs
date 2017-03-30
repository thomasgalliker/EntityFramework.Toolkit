using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Principal;
#if !NET40
using System.Threading.Tasks;
#endif

using EntityFramework.Toolkit.Auditing;
using EntityFramework.Toolkit.Auditing.Extensions;

using EntityFramework.Toolkit.Extensions;

namespace EntityFramework.Toolkit.Auditing
{
    /// <summary>
    ///     AuditDbContextBase adds auditing capabilities to the DbContextBase.
    ///     Auditing is enabled by default but may be disabled (AuditEnabled=false) if needed.
    /// </summary>
    public abstract class AuditDbContextBase<TContext> : DbContextBase<TContext>, IAuditContext where TContext : DbContext
    {
        private const string AuditUpdatedColumnName = "Audited";
        private const string AuditUserColumnName = "AuditUser";
        private const string AuditTypeColumnName = "AuditType";

        private static readonly IList<TContext> ConfigFileLock = new List<TContext>();
        private static readonly AuditDbContextConfiguration AuditDbContextConfiguration;

        private readonly Dictionary<Type, AuditTypeInfo> auditTypes = new Dictionary<Type, AuditTypeInfo>();
        private bool auditEnabled = true;

        static AuditDbContextBase()
        {
            lock (ConfigFileLock)
            {
                AuditDbContextConfiguration = AuditDbContextConfigurationManager.GetAuditDbContextConfigurationFromXml();
            }
        }

        /// <summary>
        ///     Empty constructor is used for 'update-database' command-line command.
        /// </summary>
        protected AuditDbContextBase()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the AuditDbContext class
        ///     using the given string as the name or connection string
        ///     for the database to which a connection will be made.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or the connection string.</param>
        protected AuditDbContextBase(string nameOrConnectionString, IDatabaseInitializer<TContext> databaseInitializer)
            : this(nameOrConnectionString, databaseInitializer, log: null)
        {
        }

        protected AuditDbContextBase(string nameOrConnectionString, IDatabaseInitializer<TContext> databaseInitializer, Action<string> log)
            : base(nameOrConnectionString, databaseInitializer, log)
        {
        }

        protected AuditDbContextBase(IDbConnection dbConnection, IDatabaseInitializer<TContext> databaseInitializer)
            : base(dbConnection, databaseInitializer)
        {
        }

        protected AuditDbContextBase(IDbConnection dbConnection, IDatabaseInitializer<TContext> databaseInitializer, Action<string> log)
            : base(dbConnection, databaseInitializer, log)
        {
        }

        /// <summary>
        ///     Initializes static members of the AuditDbContext class.
        /// </summary>
        protected void ConfigureAuditingFromAppConfig()
        {
            this.ConfigureAuditing(AuditDbContextConfiguration);
        }

        protected void ConfigureAuditing(AuditDbContextConfiguration configuration)
        {
            this.AuditEnabled = configuration.AuditEnabled;

            foreach (var auditTypeInfo in configuration.AuditTypeInfos)
            {
                this.RegisterAuditType(auditTypeInfo);
            }
        }

        /// <inheritdoc />
        public bool AuditEnabled
        {
            get
            {
                return this.auditEnabled;
            }
            protected set
            {
                this.auditEnabled = value;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this context is using proxies.
        /// </summary>
        public bool Proxies
        {
            get
            {
                if (this.auditTypes.Count > 0)
                {
                    var f = this.auditTypes.First();
                    var e = this.Set(f.Value.AuditableEntityType).Create();
                    return e.GetType().Namespace != f.Value.AuditableEntityType.Namespace;
                }

                return this.Configuration.ProxyCreationEnabled;
            }
        }

        /// <summary>
        ///     Registers and type for auditing.
        /// </summary>
        /// <param name="auditTypeInfo"></param>
        public void RegisterAuditType(AuditTypeInfo auditTypeInfo)
        {
            lock (this.auditTypes)
            {
                if (auditTypeInfo == null)
                {
                    throw new ArgumentNullException(nameof(auditTypeInfo));
                }

                // Extract the list of propeties to audit.
                var auditEntityType = auditTypeInfo.AuditEntityType;
                var auditEntityProperties = auditEntityType.GetProperties();

                var auditableEntityType = auditTypeInfo.AuditableEntityType;
                var auditableEntityProperties = auditableEntityType.GetProperties().ToDictionary(x => x.Name);

                if (this.auditTypes.ContainsKey(auditableEntityType))
                {
                    throw new ArgumentException($"Type {auditableEntityType.Name} is already registered for auditing.", nameof(auditableEntityType));
                }

                foreach (var property in auditEntityProperties)
                {
                    if (auditableEntityProperties.ContainsKey(property.Name))
                    {
                        if (property.PropertyType == auditableEntityProperties[property.Name].PropertyType)
                        {
                            auditTypeInfo.AuditProperties.Add(property.Name);
                        }
                    }
                }

                this.auditTypes.Add(auditableEntityType, auditTypeInfo);
            }
        }

        /// <summary>
        ///     Reloads the entity from the database overwriting any property values with values from the database.
        ///     The entity will be in the Unchanged state after calling this method.
        /// </summary>
        /// <param name="entity">The entity object to reload.</param>
        public void Reload(object entity)
        {
            this.Entry(entity).Reload();
        }

        /// <summary>
        ///     Saves all changes made in this context to the underlying database
        ///     using the current windows user for auditing.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        public override ChangeSet SaveChanges()
        {
            var username = WindowsIdentity.GetCurrent().Name;
            return this.SaveChanges(username);
        }

        /// <summary>
        ///     Saves all changes made in this context to the underlying database
        ///     using the user paramater passed for auditing.
        /// </summary>
        /// <param name="username">User name for auditing.</param>
        /// <returns>The number of objects written to the underlying database.</returns>
        public ChangeSet SaveChanges(string username)
        {
            if (!this.AuditEnabled)
            {
                return base.SaveChanges();
            }

            var audits = this.AuditChanges(username);
            try
            {
                return base.SaveChanges();
            }
            catch
            {
                // Updated failed so remove the audit entities.
                foreach (var item in audits)
                {
                    this.Set(item.GetType()).Remove(item);
                }

                throw;
            }
        }

#if !NET40
        /// <inheritdoc />
        public override Task<ChangeSet> SaveChangesAsync()
        {
            var username = WindowsIdentity.GetCurrent().Name;
            return this.SaveChangesAsync(username);
        }

        /// <summary>
        ///     Saves all changes made in this context to the underlying database
        ///     using the user paramater passed for auditing.
        /// </summary>
        /// <param name="username">User name for auditing.</param>
        /// <returns>The number of objects written to the underlying database.</returns>
        public Task<ChangeSet> SaveChangesAsync(string username)
        {
            if (!this.AuditEnabled)
            {
                return base.SaveChangesAsync();
            }

            var audits = this.AuditChanges(username);
            try
            {
                return base.SaveChangesAsync();
            }
            catch
            {
                // Updated failed so remove the audit entities.
                foreach (var item in audits)
                {
                    this.Set(item.GetType()).Remove(item);
                }

                throw;
            }
        }
#endif

        private IEnumerable<IAuditEntity> AuditChanges(string username)
        {
            // Track all audit entities created in this transaction, will be removed from context on exception.
            var audits = new List<IAuditEntity>();

            // Use the same datetime for all updates in this transaction, retrieved from server when first used.
            DateTime? dateTimeNow = null;

            // Process any auditable objects.
            foreach (var entry in this.ChangeTracker.Entries())
            {
                if (dateTimeNow.HasValue == false)
                {
                    dateTimeNow = this.Database.SqlQuery<DateTime>("SELECT GETDATE()").Single();
                }

                var creatableEntity = entry.Entity as ICreatedDate;
                if (entry.State == EntityState.Added && creatableEntity != null)
                {
                    creatableEntity.CreatedDate = dateTimeNow.Value;
                }

                var updateableEntity = entry.Entity as IUpdatedDate;
                if (entry.State == EntityState.Modified && updateableEntity != null)
                {
                    if (creatableEntity != null)
                    {
                        entry.Property<ICreatedDate>(x => x.CreatedDate).IsModified = false;
                    }
                    updateableEntity.UpdatedDate = dateTimeNow.Value;
                }

                lock (this.auditTypes)
                {
                    var entityType = entry.GetEntityType();
                    if (this.auditTypes.ContainsKey(entityType) && this.auditTypes[entityType].AuditEntityType != null)
                    {
                        var auditEntity = this.AuditEntity(entry, this.auditTypes[entityType], dateTimeNow.Value, username);
                        audits.Add(auditEntity);
                    }
                }
            }

            return audits;
        }

        private IAuditEntity AuditEntity(DbEntityEntry entityEntry, AuditTypeInfo auditTypeInfo, DateTime auditDateTime, string user)
        {
            // Create audit entity.
            var dbSet = this.Set(auditTypeInfo.AuditEntityType);
            var auditEntity = (IAuditEntity)dbSet.Create();
            dbSet.Add(auditEntity);

            // Copy the properties.
            var auditEntityEntry = this.Entry(auditEntity);
            if (entityEntry.State == EntityState.Added)
            {
                foreach (string propertyName in auditTypeInfo.AuditProperties)
                {
                    auditEntityEntry.Property(propertyName).CurrentValue = entityEntry.Property(propertyName).CurrentValue;
                }
            }
            else
            {
                foreach (string propertyName in auditTypeInfo.AuditProperties)
                {
                    auditEntityEntry.Property(propertyName).CurrentValue = entityEntry.Property(propertyName).OriginalValue;
                }
            }

            // Set the audit columns.
            auditEntityEntry.Property(AuditUpdatedColumnName).CurrentValue = auditDateTime;
            auditEntityEntry.Property(AuditUserColumnName).CurrentValue = user;
            auditEntityEntry.Property(AuditTypeColumnName).CurrentValue = entityEntry.State.ToAuditEntityState();

            return auditEntity;
        }
    }
}