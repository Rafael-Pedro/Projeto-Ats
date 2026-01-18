using Ats.Application.UseCases.Candidates;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Presentation.Serialization;

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
    public async Task<IActionResult> Create([FromBody] CreateCandidateCommand request)
    {
        var result = await _sender.Send(request);

        return HttpSerialization.Serialize(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var query = new GetCandidateByIdQuery(id);
        var result = await _sender.Send(query);

        return HttpSerialization.Serialize(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetAllCandidatesQuery(page, pageSize);
        var result = await _sender.Send(query);

        return HttpSerialization.Serialize(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCandidateCommand request)
    {
        if (id != request.Id)
            return BadRequest("O ID da URL não coincide com o ID do corpo da requisição.");

        var result = await _sender.Send(request);

        return HttpSerialization.Serialize(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _sender.Send(new DeleteCandidateCommand(id));

        return HttpSerialization.Serialize(result);
    }
}