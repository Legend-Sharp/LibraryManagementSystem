using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Features.Members.Commands;
using LibraryManagementSystem.Application.Features.Members.DTOs;
using LibraryManagementSystem.Application.Features.Members.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MemberDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<MemberDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
        => Ok(await mediator.Send(new GetMembersQuery(page, pageSize, search)));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDto>> GetById(Guid id)
    {
        var dto = await mediator.Send(new GetMemberByIdQuery(id));
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateMemberCommand cmd)
    {
        var id = await mediator.Send(cmd);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }
}