using LibraryManagementSystem.Application.Members.Commands;
using LibraryManagementSystem.Application.Members.DTOs;
using LibraryManagementSystem.Application.Members.Queries;
using LibraryManagementSystem.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateMemberCommand cmd)
        => Ok(await mediator.Send(cmd));

    [HttpGet]
    public async Task<ActionResult<PagedResult<MemberDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
        => Ok(await mediator.Send(new GetMembersQuery(page, pageSize, search)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MemberDto?>> GetById(Guid id)
        => Ok(await mediator.Send(new GetMemberByIdQuery(id)));
}