using Ats.Application.UseCases.Jobs;
using Ats.WebAPI.Serialization;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Ats.WebAPI.Controllers;

[ApiController]
[Route("api/Jobs")]
public class JobsController : ControllerBase
{
    private readonly ISender _sender;

    public JobsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobCommand request)
    {
        var result = await _sender.Send(request);

        return HttpSerialization.Serialize(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool onlyActive = false
    )
    {
        var query = new GetAllJobsQuery(page, pageSize, onlyActive);

        var result = await _sender.Send(query);

        return HttpSerialization.Serialize(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var query = new GetJobByIdQuery(id);
        var result = await _sender.Send(query);
        return HttpSerialization.Serialize(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateJobRequest request)
    {
        var command = new UpdateJobCommand(id, request.Title, request.Description, request.Salary);
        var result = await _sender.Send(command);
        return HttpSerialization.Serialize(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteJobCommand(id);
        var result = await _sender.Send(command);
        return HttpSerialization.Serialize(result);
    }

    public record UpdateJobRequest(string Title, string Description, decimal? Salary);
}