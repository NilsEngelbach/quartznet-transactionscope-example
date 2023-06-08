using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace TransactionScopeExample;

public class ExampleDbContext : DbContext
{
    public static string ConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres;";

    public DbSet<ExampleEntity> ExampleEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddQuartz(builder => builder.UsePostgreSql());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(ConnectionString);
}