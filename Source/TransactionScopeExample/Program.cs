using Quartz;
using Quartz.Impl.AdoJobStore;
using TransactionScopeExample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddTransient<ExampleService>();

builder.Services.AddDbContext<ExampleDbContext>();

builder.Services.AddScoped<ExampleDbContextJobStore>();

builder.Services.AddScoped<IExampleDbContextJobStore>(sp =>
{
    var dbContext = sp.GetRequiredService<ExampleDbContext>();
    return new ExampleDbContextJobStore(dbContext);
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();

    q.UsePersistentStore(c =>
    {
        c.Properties.Add("quartz.jobStore.type", "TransactionScopeExample.IExampleDbContextJobStore");
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
