using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Features.Members.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Members.Queries;

public sealed record GetMemberByIdQuery(Guid Id) : IRequest<MemberDto?>;

public sealed class GetMemberByIdHandler(IAppDbContext db) : IRequestHandler<GetMemberByIdQuery, MemberDto?>
{
    public async Task<MemberDto?> Handle(GetMemberByIdQuery r, CancellationToken ct)
        => await db.Members.AsNoTracking()
            .Where(m => m.Id == r.Id)
            .Select(m => new MemberDto(m.Id, m.Name, m.Email))
            .FirstOrDefaultAsync(ct);
}