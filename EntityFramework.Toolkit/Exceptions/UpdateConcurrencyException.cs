using System;

/// <summary>
/// Exception thrown by <see cref="T:System.Data.Extension.DbContextBase{TContext}" /> when the database 
/// has been concurrently updated such that a concurrency token that was expected to match did not actually match.
/// </summary>
public class UpdateConcurrencyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateConcurrencyException" /> class.
    /// </summary>
    public UpdateConcurrencyException(object conflictingEntity, object databaseEntity) 
        : base(string.Format(""))
    {
        this.ConflictingEntity = conflictingEntity;
        this.DatabaseEntity = databaseEntity;
    }

    public object ConflictingEntity { get; private set; }

    public object DatabaseEntity { get; private set; }
}