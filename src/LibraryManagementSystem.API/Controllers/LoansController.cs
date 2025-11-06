using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Features.Loans.Commands;
using LibraryManagementSystem.Application.Features.Loans.Dtos;
using LibraryManagementSystem.Application.Features.Loans.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LoanListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<LoanListItemDto>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? memberId = null,
        [FromQuery] Guid? bookId = null,
        [FromQuery] bool? active = null)
        => Ok(await mediator.Send(new GetLoansQuery(page, pageSize, memberId, bookId, active)));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LoanListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanListItemDto>> GetById(Guid id)
    {
        var dto = await mediator.Send(new GetLoanByIdQuery(id));
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("borrow")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Borrow([FromBody] BorrowBookForMemberCommand cmd)
    {
        var loanId = await mediator.Send(cmd);
        return CreatedAtAction(nameof(GetById), new { id = loanId }, loanId);
    }

    [HttpPost("{loanId:guid}/return")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Return(Guid loanId)
    {
        await mediator.Send(new ReturnBookForMemberCommand(loanId));
        return NoContent();
    }
}