
# 📚 Library Management System (Clean Architecture + CQRS + .NET 9)

A production-grade backend API demonstrating Clean Architecture, CQRS using MediatR, EF Core (Code‑First), FluentValidation, caching, domain events, and automated tests.

> ✅ Built for a technical assignment — focused on clean architecture, SOLID, maintainability, separation of concerns, and realistic enterprise patterns.

---

## ✅ Features

| Area | Implementation |
|------|---------------|
| Architecture | Clean Architecture: Domain → Application → Infrastructure → API |
| CQRS | Commands & Queries using MediatR |
| Persistence | SQL Server + EF Core Code‑First |
| Domain Model | `Book`, `Member`, `Loan` (business rules enforced in domain) |
| Domain Events | Borrow/Return raises domain events → event handlers update availability |
| Concurrency | `RowVersion` prevents double‑borrowing race conditions |
| Caching | Automatic caching for GET queries using MediatR pipeline |
| Validation | FluentValidation + validation pipeline behavior |
| Performance | Pagination + caching + async everywhere |
| Testing | Domain + Application tests (EF InMemory) |
| Documentation | Swagger UI with models + request/response schemas |

---

## 🧱 Architecture

```
src
 ├── LibraryManagementSystem.API            → Controllers, DI setup, middleware
 ├── LibraryManagementSystem.Application    → CQRS (Commands / Queries), pipeline behaviors, handlers
 ├── LibraryManagementSystem.Domain         → Entities, ValueObjects, Domain Events (no deps)
 └── LibraryManagementSystem.Infrastructure → DbContext, EF Core, persistence, SQL Server

tests
 └── LibraryManagementSystem.Tests          → Domain + Application tests (EF InMemory)
```

Principles followed:
- Dependency Rule (Domain has no dependencies)
- No business logic in controllers
- Command / Query separation

---

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server installed or LocalDB

---

### 1️⃣ Clone

```sh
git clone <your_repo_url>
cd LibraryManagementSystem
```

---

### 2️⃣ Configure DB Connection

Modify:
`src/LibraryManagementSystem.API/appsettings.json`

```json
"ConnectionStrings": {
  "LibraryDb": "Server="<YOUR_SQL_SERVER_CONNECTION_STRING>";Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

👉 Replace `"<YOUR_SQL_SERVER_CONNECTION_STRING>"` with your SQL Server instance.

---

### 3️⃣ Apply migrations

```sh
dotnet ef migrations add InitialCreate -p ./src/LibraryManagementSystem.Infrastructure -s ./src/LibraryManagementSystem.API
dotnet ef database update -p ./src/LibraryManagementSystem.Infrastructure -s ./src/LibraryManagementSystem.API
```

---

### 4️⃣ Run

```sh
dotnet run --project ./src/LibraryManagementSystem.API
```

Swagger:
http://localhost:5134/swagger/index.html

---

## 🔥 Endpoints Overview

### Books
- GET  `/api/books`
- POST `/api/books`
- POST `/api/books/bulk-import`

### Members
- POST `/api/members`
- GET  `/api/members`
- GET  `/api/members/{id}/loans`

### Loans
- POST `/api/loans/borrow`
- POST `/api/loans/{loanId}/return`
- GET  `/api/loans?memberId=&active=`

---

## ✅ Testing

Run tests:

```sh
dotnet test
```

Included tests:
- domain behavior (`Borrow`, `Return`, edge cases)
- application tests using EF InMemory
- validators and paged queries

---

## 🏁 Final Notes

This project demonstrates clean separation of concerns, domain‑driven thinking, and production‑ready CQRS patterns.

