using Ats.Application.UseCases.Candidates;
using Ats.WebAPI.Serialization;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Ats.WebAPI.Controllers;

[ApiController]
[Route("api/candidates")]
public class CandidateController : ControllerBase
{
    private readonly ISender _sender;

    public CandidateController(ISender mediator)
    {
        _sender = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateCandidateRequest request)
    {
        byte[]? fileBytes = null;
        string? fileName = null;

        if (request.ResumeFile != null && request.ResumeFile.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await request.ResumeFile.CopyToAsync(memoryStream);
            fileBytes = memoryStream.ToArray();
            fileName = request.ResumeFile.FileName;
        }

        var command = new CreateCandidateCommand(
            request.Name,
            request.Email,
            request.Age,
            request.LinkedIn,
            fileBytes,
            fileName
        );

        var result = await _sender.Send(command);

        return HttpSerialization.Serialize(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var query = new GetCandidateByIdQuery(id);
        var result = await _sender.Send(query);

        return HttpSerialization.Serialize(result);
    }

    [HttpGet("{id:guid}/resume")]
    public async Task<IActionResult> DownloadResume([FromRoute] Guid id)
    {
        var query = new DownloadCandidateResumeQuery(id);
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HttpSerialization.Serialize(result);

        var fileData = result.Value;

        return File(fileData.FileContent, "application/octet-stream", fileData.FileName);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetAllCandidatesQuery(page, pageSize);
        var result = await _sender.Send(query);

        return HttpSerialization.Serialize(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
    [FromRoute] Guid id,
    [FromForm] UpdateCandidateRequest request
)
    {
        byte[]? fileBytes = null;
        string? fileName = null;

        if (request.ResumeFile != null && request.ResumeFile.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await request.ResumeFile.CopyToAsync(memoryStream);
            fileBytes = memoryStream.ToArray();
            fileName = request.ResumeFile.FileName;
        }

        var command = new UpdateCandidateCommand(
            id,
            request.Name,
            request.Email,
            request.Age,
            request.LinkedIn,
            fileBytes,
            fileName
        );

        var result = await _sender.Send(command);

        return HttpSerialization.Serialize(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Disable([FromRoute] Guid id)
    {
        var result = await _sender.Send(new DisableCandidateCommand(id));

        return HttpSerialization.Serialize(result);
    }

    public record CreateCandidateRequest(
    string Name,
    string Email,
    int Age,
    string? LinkedIn,
    IFormFile? ResumeFile
);

    public record UpdateCandidateRequest(
    string? Name,
    string? Email,
    int? Age,
    string? LinkedIn,
    IFormFile? ResumeFile
);
}