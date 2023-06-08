using Quartz;
using Quartz.Impl.AdoJobStore;
using TransactionScopeExample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddTransient<ExampleService>();

builder.Services.AddDbContext<ExampleDbContext>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();

    q.UsePersistentStore(c =>
    {
        c.UseProperties = true;
        c.UseJsonSerializer();
        c.UsePostgres(b => {
            b.UseDriverDelegate<PostgreSQLDelegate>();
            b.ConnectionString = ExampleDbContext.ConnectionString;
            b.TablePrefix = "quartz.qrtz_";
        });
    });
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

app.MapControllers();

app.Run();
