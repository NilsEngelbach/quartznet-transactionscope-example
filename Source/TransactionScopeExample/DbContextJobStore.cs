using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Quartz.Impl.AdoJobStore;
using Quartz.Util;

namespace TransactionScopeExample;


public class ExampleDbContextJobStore : DbContextJobStore<ExampleDbContext>
{
    public ExampleDbContextJobStore(ExampleDbContext dbContext) : base(dbContext){ }
}

/// <summary>
/// Subclass of Quartz's JobStoreCMT class that delegates to a Entity Framework managed
/// DbContext instead of using a Quartz-managed connection pool.
/// </summary>
/// <remarks>
/// <p>Operations performed by this JobStore will properly participate in any
/// kind of TransactionScoped transaction, as it uses Entity Frameworks Database
/// Connection handling methods that are aware of a current transaction.</p>
///
/// <p>Note that all Quartz Scheduler operations that affect the persistent
/// job store should usually be performed within active transactions,
/// as they assume to get proper locks etc.</p>
/// </remarks>
public class DbContextJobStore<T> : JobStoreCMT where T : DbContext
{
    private readonly T _dbContext;

    public DbContextJobStore(T dbContext)
    {
        _dbContext = dbContext;
    }

    public override string InstanceName
    {
        get => base.InstanceName;
        set
        {
            base.InstanceName = value;
            DBConnectionManager.Instance.AddConnectionProvider(InstanceName, new DbContextProviderAdapter(_dbContext));
        }
    }

    protected override ConnectionAndTransactionHolder GetNonManagedTXConnection()
    {
        var dbConnection = _dbContext.Database.GetDbConnection();
        var transaction = _dbContext.Database.CurrentTransaction;
        return new ConnectionAndTransactionHolder(dbConnection, transaction?.GetDbTransaction());
    }

    protected override void CloseConnection(ConnectionAndTransactionHolder connectionAndTransactionHolder)
    {
        connectionAndTransactionHolder.Connection.Close();
    }
}