using System.Transactions;
using Quartz;

namespace TransactionScopeExample;

public class ExampleService
{
    private readonly ExampleDbContext _dbContext;
    private readonly ISchedulerFactory _schedulerFactory;

    public ExampleService(ISchedulerFactory schedulerFactory, ExampleDbContext dbContext)
    {
        _dbContext = dbContext;
        _schedulerFactory = schedulerFactory;
    }

    // Not working: Making changes and add job in same transaction
    public async Task WithEFTransaction()
    {
        using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
        {
            // Make changes to entities
            var entity = _dbContext.ExampleEntities.Add(new ExampleEntity() { Name = new Random().Next(1, 100).ToString() });
            await _dbContext.SaveChangesAsync();

            // Add job for entity
            var scheduler = await _schedulerFactory.GetScheduler();
            
            var job = JobBuilder.Create<ExampleJob>()
                .WithIdentity("exampleJob")
                .UsingJobData("entity-name", entity.Entity.Name)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("exampleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            dbContextTransaction.Commit();
        }
    }

    // Not working: Making changes and add job in same transaction
    public async Task WithTransactionScope()
    {
        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            // Make changes to entities
            var entity = _dbContext.ExampleEntities.Add(new ExampleEntity() { Name = new Random().Next(1, 100).ToString() });
            await _dbContext.SaveChangesAsync();

            // Add job for entity
            var scheduler = await _schedulerFactory.GetScheduler();
            
            var job = JobBuilder.Create<ExampleJob>()
                .WithIdentity("exampleJob")
                .UsingJobData("entity-name", entity.Entity.Name)
                .Build();
                
            var trigger = TriggerBuilder.Create()
                .WithIdentity("exampleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            transaction.Complete();
        }
    }

    // This is working - but not same transaction
    public async Task WithSuppredTransactionScope()
    {
        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            // Make changes to entities
            var entity = _dbContext.ExampleEntities.Add(new ExampleEntity() { Name = new Random().Next(1, 100).ToString() });
            await _dbContext.SaveChangesAsync();

            // Add job for entity
            var scheduler = await _schedulerFactory.GetScheduler();

            var job = JobBuilder.Create<ExampleJob>()
                .WithIdentity("exampleJob")
                .UsingJobData("entity-name", entity.Entity.Name)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("exampleTrigger")
                .StartNow()
                .Build();
            
            using (var suppressedTransactionScope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await scheduler.ScheduleJob(job, trigger);
            }

            transaction.Complete();
        }
    }

    // This is working - but not same transaction
    public async Task WithoutTransaction()
    {
        // Make changes to entities
        var entity = _dbContext.ExampleEntities.Add(new ExampleEntity() { Name = new Random().Next(1, 100).ToString() });
        await _dbContext.SaveChangesAsync();

        // Add job for entity
        var scheduler = await _schedulerFactory.GetScheduler();

        var job = JobBuilder.Create<ExampleJob>()
            .WithIdentity("exampleJob")
            .UsingJobData("entity-name", entity.Entity.Name)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("exampleTrigger")
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}