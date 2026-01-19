using Ats.Application.UseCases.JobApplications;
using Ats.WebAPI.Serialization;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Ats.WebAPI.Controllers;

[ApiController]
[Route("api/job-applications")]
public class JobApplicationsController : ControllerBase
{
    private readonly ISender _sender;

    public JobApplicationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Apply([FromBody] ApplyToJobCommand command)
    {
        var result = await _sender.Send(command);
        return HttpSerialization.Serialize(result);
    }

    [HttpGet("job/{jobId:guid}")]
    public async Task<IActionResult> GetByJob([FromRoute] Guid jobId)
    {
        var query = new GetApplicationsByJobQuery(jobId);
        var result = await _sender.Send(query);
        return HttpSerialization.Serialize(result);
    }

    [HttpGet("candidate/{candidateId:guid}")]
    public async Task<IActionResult> GetByCandidate([FromRoute] Guid candidateId)
    {
        var query = new GetApplicationsByCandidateQuery(candidateId);
        var result = await _sender.Send(query);
        return HttpSerialization.Serialize(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus([FromRoute] Guid id, [FromBody] string action)
    {
        var result = await _sender.Send(new ChangeApplicationStatusCommand(id, action));
        return HttpSerialization.Serialize(result);
    }
}