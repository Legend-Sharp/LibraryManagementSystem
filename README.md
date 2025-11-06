# 📚 Library Management System (Clean Architecture + CQRS + .NET 9)

A production-grade backend API demonstrating **Clean Architecture**, **CQRS (MediatR)**, **EF Core (Code-First)**, **FluentValidation**, **domain events**, **caching**, **optimistic concurrency**, and **automated tests**.

> ✅ Built for a assignment — focused on SOLID, maintainability, separation of concerns, and realistic enterprise patterns.

---

## ✅ Features

| Area | Implementation |
|------|----------------|
| Architecture | Clean Architecture: **Domain → Application → Infrastructure → API** |
| CQRS | Commands/Queries via **MediatR** + pipeline behaviors (Validation, Caching) |
| Persistence | **SQL Server** + **EF Core** (code-first, migrations) |
| Domain Model | `Book`, `Member`, `Loan` with business rules inside the domain |
| Domain Events | Borrow/Return raise events; handlers update book availability |
| Concurrency | **RowVersion** & safe inventory math to avoid lost updates |
| Soft Delete | `Book.IsDeleted` + **global query filter** (books disappear from queries) |
| Caching | Short-lived GET caching via MediatR behavior |
| Validation | **FluentValidation** + validation behavior |
| Performance | Pagination on list endpoints + async EF queries |
| Dev UX | **Auto-migrate** and optional **seed data** on startup (configurable) |
| Testing | Domain & Application tests (EF InMemory) |
| Docs | Swagger UI with models and request/response schemas |

---

## 🧱 Architecture

```
src
 ├─ LibraryManagementSystem.API            → Controllers, DI, middleware, Swagger
 ├─ LibraryManagementSystem.Application    → CQRS (Commands/Queries), DTOs, Behaviors
 ├─ LibraryManagementSystem.Domain         → Entities, ValueObjects, Domain Events (no deps)
 └─ LibraryManagementSystem.Infrastructure → DbContext, EF config, migrations, seeding

tests
 └─ LibraryManagementSystem.Tests          → Domain + Application tests (EF InMemory)
```

Principles:
- Dependency Rule (Domain has **no** external dependencies)
- No business logic in controllers (thin endpoints)
- Clear Command/Query separation

---

## 🚀 Getting Started

### Prerequisites
- **.NET 9 SDK**
- **SQL Server** (Developer/Express/LocalDB)

---

### 1) Clone

```bash
git clone https://github.com/Legend-Sharp/LibraryManagementSystem
cd LibraryManagementSystem
```

---

### 2) Configure connection string & startup flags

Modify `src/LibraryManagementSystem.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "AutoMigrate": true,
  "SeedOnStartup": true
}
```

---

### 3) Migrations

```bash
dotnet ef migrations add InitialCreate -p ./src/LibraryManagementSystem.Infrastructure -s ./src/LibraryManagementSystem.API
```

---

### 4) Run the API

```bash
dotnet run --project ./src/LibraryManagementSystem.API
```

Swagger URL:
```
http://localhost:<port>/swagger
```

---

## 🔥 Endpoints Overview

### Books
| Method | URL |
|--------|-----|
| GET    | `/api/books` |
| GET    | `/api/books/{id}` |
| POST   | `/api/books` |
| PUT    | `/api/books/{id}` |
| DELETE | `/api/books/{id}` |
| POST   | `/api/books/bulk-import` |

### Members
| Method | URL |
|--------|-----|
| GET    | `/api/members` |
| GET    | `/api/members/{id}` |
| POST   | `/api/members` |

### Loans
| Method | URL |
|--------|-----|
| GET    | `/api/loans?memberId=&bookId=&active=` |
| GET    | `/api/loans/{id}` |
| POST   | `/api/loans/borrow` |
| POST   | `/api/loans/{loanId}/return` |

---

## ✅ Testing

Run tests:

```bash
dotnet test
```

Tests include:
- Domain tests (borrow, return, edge cases)
- Application tests using EF InMemory
- Validator tests
- Query pagination

---

## 🏁 Final Notes

- Thin controllers, rich domain model, and clear separation with CQRS.
- Auto-migration + optional seed data enables quick setup.
- Designed for real-world maintainability and extensibility.

---

Enjoy building ✅🚀
