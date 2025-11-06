using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Features.Books.Commands;
using LibraryManagementSystem.Application.Features.Books.DTOs;
using LibraryManagementSystem.Application.Features.Books.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BookDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<BookDto>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
        => Ok(await mediator.Send(new GetBooksQuery(page, pageSize, search)));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetById(Guid id)
    {
        var dto = await mediator.Send(new GetBookByIdQuery(id));
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookCommand cmd)
    {
        var created = await mediator.Send(cmd);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("bulk-import")]
    [ProducesResponseType(typeof(BulkImportResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkImportResult>> BulkImport([FromBody] BulkImportBooksCommand cmd)
        => Ok(await mediator.Send(cmd));
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> Update(Guid id, [FromBody] UpdateBookCommand cmd)
    {
        if (id != cmd.Id) return BadRequest("Route id and payload id must match.");
        var updated = await mediator.Send(cmd);
        return Ok(updated);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteBookCommand(id));
        return NoContent();
    }
}