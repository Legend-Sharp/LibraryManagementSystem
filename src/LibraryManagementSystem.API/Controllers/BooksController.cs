// src/LibraryManagementSystem.API/Controllers/BooksController.cs
using LibraryManagementSystem.Application.Books.Commands;
using LibraryManagementSystem.Application.Books.DTOs;
using LibraryManagementSystem.Application.Books.Queries;
using LibraryManagementSystem.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<BookDto>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
        => Ok(await mediator.Send(new GetBooksQuery(page, pageSize, search)));

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookCommand cmd)
        => Ok(await mediator.Send(cmd));

    [HttpPost("bulk-import")]
    public async Task<ActionResult<BulkImportResult>> BulkImport([FromBody] BulkImportBooksCommand cmd)
        => Ok(await mediator.Send(cmd));
}