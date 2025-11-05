using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Common;
using LibraryManagementSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Infrastructure.Persistence;

public class LibraryDbContext : DbContext, IAppDbContext
{
    private readonly IPublisher _publisher;

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options, IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Book>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Author).HasMaxLength(200).IsRequired();
            e.OwnsOne(x => x.Isbn, nb =>
            {
                nb.Property(p => p.Value).HasColumnName("Isbn").HasMaxLength(13).IsRequired();
                nb.HasIndex(p => p.Value).IsUnique();
            });
            e.Property(x => x.TotalCopies).IsRequired();
            e.Property(x => x.AvailableCopies).IsRequired();
            e.Property<byte[]>("RowVersion").IsRowVersion().HasColumnName("RowVersion");
        });

        b.Entity<Member>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
        });

        b.Entity<Loan>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.MemberId, x.BookId, x.BorrowedAt });
            e.Property(x => x.BorrowedAt).IsRequired();

            e.HasOne<Member>()
                .WithMany()
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne<Book>()
                .WithMany()
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // 1) find all entities with domain events
        var domainEntities = ChangeTracker.Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        // 2) clear before dispatching
        domainEntities.ForEach(x => x.Entity.ClearDomainEvents());

        // 3) publish via MediatR
        foreach (var domainEvent in domainEvents)
        {
            var notification = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());

            await _publisher.Publish(
                (INotification)Activator.CreateInstance(notification, domainEvent)!,
                ct
            );
        }

        return await base.SaveChangesAsync(ct);
    }
}