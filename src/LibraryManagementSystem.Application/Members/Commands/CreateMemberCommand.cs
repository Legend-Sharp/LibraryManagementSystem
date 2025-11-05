using FluentValidation;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Members.Commands;

public sealed record CreateMemberCommand(string Name, string Email) : IRequest<Guid>;

public sealed class CreateMemberValidator : AbstractValidator<CreateMemberCommand>
{
    public CreateMemberValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
    }
}

public sealed class CreateMemberHandler(IAppDbContext db) : IRequestHandler<CreateMemberCommand, Guid>
{
    public async Task<Guid> Handle(CreateMemberCommand request, CancellationToken ct)
    {
        var exists = await db.Members.AnyAsync(m => m.Email == request.Email, ct);
        if (exists) throw new InvalidOperationException("Email already registered.");

        var member = Member.Create(request.Name, request.Email);
        
        await db.Members.AddAsync(member, ct);
        await db.SaveChangesAsync(ct);
        
        return member.Id;
    }
}