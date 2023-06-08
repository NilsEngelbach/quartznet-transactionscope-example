using Quartz;

namespace TransactionScopeExample;

public class ExampleJob : IJob
{
    private readonly ILogger<ExampleJob> _logger;

    public ExampleJob(ILogger<ExampleJob> logger)
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        try 
        {
            var name = context.MergedJobDataMap.GetString("entity-name");
            _logger.LogInformation($"Running ExampleJob for entity {name}");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: $"Error running ExampleJob", refireImmediately: false, cause: ex);
        }
    }
}