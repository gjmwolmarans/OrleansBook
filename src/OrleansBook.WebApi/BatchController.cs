using Microsoft.AspNetCore.Mvc;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.WebApi;

[ApiController]
public class BatchController : ControllerBase
{
    private readonly IClusterClient _client;
    public BatchController(IClusterClient client) => _client = client;

    [HttpPost]
    [Route("batch")]
    public async Task<IActionResult> Post(IDictionary<string, string> values)
    {
        var grain = _client.GetGrain<IBatchGrain>(0);
        var input = values.Select(keyValue => (keyValue.Key, keyValue.Value))
            .ToArray();
        await grain.AddInstructions(input);
        return Ok();
    }
}
