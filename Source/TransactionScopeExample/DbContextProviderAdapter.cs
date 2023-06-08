using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Quartz.Impl.AdoJobStore.Common;

namespace TransactionScopeExample;

public class DbContextProviderAdapter : IDbProvider
{
    private readonly DbContext _dbContext;
    private readonly DbMetadata _metadata;

    public DbContextProviderAdapter(DbContext dbContext)
    {
        _dbContext = dbContext;
        _metadata = new DbContextMetadataAdapter(dbContext);
    }

    public void Initialize() { }

    public DbCommand CreateCommand()
    {
        return _dbContext.Database.GetDbConnection().CreateCommand();
    }

    public DbConnection CreateConnection()
    {
        return _dbContext.Database.GetDbConnection();
    }

    public void Shutdown() { }

    public string ConnectionString
    {
        get => _dbContext.Database.GetConnectionString();
        set => _dbContext.Database.SetConnectionString(value); // TODO: Really?
    }

    public DbMetadata Metadata => _metadata;
}

/// <summary>
/// Helper class to map between Quartz and DbContext metadata.
/// </summary>
public class DbContextMetadataAdapter : DbMetadata
{
    private readonly DbContext _dbContext;

    public DbContextMetadataAdapter(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
}