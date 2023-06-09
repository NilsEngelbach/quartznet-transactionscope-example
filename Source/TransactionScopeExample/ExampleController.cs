using Microsoft.AspNetCore.Mvc;

namespace TransactionScopeExample;

[ApiController]
[Route("[controller]")]
public class ExampleController : ControllerBase
{
    private readonly ILogger<ExampleController> _logger;
    private ExampleService _exampleService;

    public ExampleController(ILogger<ExampleController> logger, ExampleService exampleService)
    {
        _logger = logger;
        _exampleService = exampleService;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        await _exampleService.WithEFTransaction();
        return Ok();
    }
}
