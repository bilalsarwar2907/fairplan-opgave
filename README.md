# Fairplan – Mini App (Take-Home Assignment)

Take-home assignment for the **software developer praktik (spring 2027) at Computation ApS / Fairplan**.
Made by Bilal Sarwar, datamatiker student at Zealand.

A full-stack application for managing periods, sections, staff groups, users, and employments. It also includes a read-only staff overview for a selected period.

**Backend:** C# / .NET 8 Web API, EF Core, SQLite, xUnit
**Frontend:** React, TypeScript, React Router v7, Vite, Tailwind

---

## Run the Application

### Requirements

- .NET SDK 9 (configured in `backend/global.json`; builds .NET 8)
- Node.js 20+

### Backend

- Runs on: http://localhost:5166
- Swagger: http://localhost:5166/swagger

```powershell
cd backend
dotnet run --project src/WebAPI
```

### Frontend

- Runs on: http://localhost:5173

```powershell
cd frontend
npm install
npm run dev
```

### Run Tests

From the `backend` folder:

```powershell
dotnet test
```

There are **21 automated tests**.

The SQLite database is created, migrated and seeded with demo data automatically when the backend starts.

**Database file:** `backend/src/WebAPI/fairplan.db` – delete it to reset the demo data.

---

## Project Structure (onion architecture)

| Folder | Contains |
|---|---|
| `backend/src/Core` | Domain entities: Period, Section, StaffGroup, User, Employment. Depends on no other layer. |
| `backend/src/Application` | Services with all business rules, DTOs, `IAppDbContext`, `BusinessException` |
| `backend/src/Infrastructure` | `AppDbContext`, EF Core configuration, SQLite, migrations, seed data |
| `backend/src/WebAPI` | Controllers, `ExceptionMiddleware`, `Program.cs` (dependency injection + pipeline) |
| `backend/Tests` | xUnit tests using in-memory SQLite |
| `frontend/app` | `services/apiService.ts`, `components/DataTable.tsx`, `components/forms/*`, `routes/*` |

Dependencies always point inward. Application services depend on `IAppDbContext`; Infrastructure provides the implementation.

---

## Business Rules (enforced in the backend services)

**Periods**
- P1 Name, start date and end date are required.
- P2 Start date must be before end date.
- P3 Periods cannot overlap. Adjacent periods are allowed.

**Sections**
- A1 Name is required. Colour must be HEX (`#RRGGBB`).
- A2 A section cannot be deleted if it has employments.

**Staff groups**
- G1 Name is required.
- G2 A staff group cannot be deleted if it has employments.

**Users**
- U1 Name and a valid email are required.
- U2 Email must be unique (case-insensitive).
- U3 A user cannot be deleted if they have employments.
- U4 A user must have at least one role (Admin or Personale). The Personale role cannot be removed while the user has employments.

**Employments**
- E1 Hours per week must be between 0 and 37.
- E2 End date must be after start date.
- E3 The user must have the Personale role.
- E4 One user's employments cannot overlap.
- E5 User, section and staff group must exist.

**Staff view**
- S1 Shows all employments that overlap the selected period.

All dates are inclusive. One helper, `DateRangeHelper.Overlaps`, is used for P3, E4 and S1.
Errors are returned as `ProblemDetails` with Danish messages and shown in the UI.

---

## Key Decisions

- **Business rules live in the services, not the controllers.** Controllers stay thin, and the rules are tested in one place.
- **No custom repositories.** EF Core already works as Repository + Unit of Work. `IAppDbContext` keeps the services decoupled and testable.
- **Centralised error handling.** `BusinessException` + `ExceptionMiddleware` give the frontend one consistent error format (400 / 404 / 409).
- **Database safety net.** Foreign keys with `Restrict` and a unique index on email.
- **SQLite** so reviewers can run the app with no database setup.
- **Tests use real in-memory SQLite** instead of mocks, so foreign keys and indexes behave like in production.
- **One reusable `DataTable` component** handles search, sorting and filtering for all six tables.
- **A Vite proxy forwards `/api`** to the backend, so no CORS setup is needed.

---

## Simplifications

Made on purpose because of the assignment scope and time frame:

- No authentication or authorisation.
- No pagination.
- Delete confirmation uses `window.confirm`.
- Search works on raw values (e.g. ISO dates, not the displayed dd-mm-yyyy).
- Empty dropdown or date fields in the employment form show a generic validation message.
- `npm audit` warnings from the template's dependencies were not fixed before the deadline.

---

## What I Learned

- End-to-end testing in the browser found two bugs that the unit tests could not see (a page loading the wrong endpoint, and ASP.NET rejecting empty strings before my own validation ran).
- How much simpler the code gets when every rule lives in one service layer and every error has one shape.

## What I Would Do Next

- Add login and roles, so only Admin users can change data.
- Add frontend tests and run all tests in a CI pipeline.
- Fix the `npm audit` warnings.
- Add pagination if the data grows.
