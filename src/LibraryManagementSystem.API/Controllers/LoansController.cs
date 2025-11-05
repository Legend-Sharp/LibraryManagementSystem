// src/LibraryManagementSystem.API/Controllers/LoansController.cs

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
    public async Task<ActionResult<PagedResult<LoanListItemDto>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? memberId = null,
        [FromQuery] Guid? bookId = null,
        [FromQuery] bool? active = null)
        => Ok(await mediator.Send(new GetLoansQuery(page, pageSize, memberId, bookId, active)));
    
    [HttpPost("borrow")]
    public async Task<ActionResult<Guid>> Borrow([FromBody] BorrowBookForMemberCommand cmd)
        => Ok(await mediator.Send(cmd));

    [HttpPost("{loanId}/return")]
    public async Task<IActionResult> Return(Guid loanId)
    {
        await mediator.Send(new ReturnBookForMemberCommand(loanId));
        return NoContent();
    }
}