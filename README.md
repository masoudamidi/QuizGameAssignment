# QuizGame — GBFS Bike Sharing Trivia

A timed trivia game built on live bike-sharing data from Oslo, New York, and Paris. Answer as many questions as you can in 60 seconds — each correct answer earns 50 points, each wrong answer costs 20. Stay positive to win.

---

## How to run

**Prerequisites:** .NET 10 SDK, PostgreSQL

### 1. Set up the database

In pgAdmin or psql:

```sql
CREATE USER gbfsquiz WITH PASSWORD 'gbfsquiz';
CREATE DATABASE gbfsquiz OWNER gbfsquiz;
```

### 2. Install the EF Core CLI tool

```bash
dotnet tool install --global dotnet-ef
```

### 3. Create the schema

```bash
cd src/QuizGame.Api
dotnet ef migrations add Init --project ../QuizGame.Infrastructure
dotnet ef database update
```

### 4. Run

```bash
dotnet run
```

Open `http://localhost:5188` in your browser. Register an account and start playing.

### 5. Run tests

```bash
dotnet test
```

---

## Project structure

```
QuizGame/
├── src/
│   ├── QuizGame.Domain/          # Entities, value objects, domain services
│   ├── QuizGame.Application/     # Use cases, interfaces, DTOs
│   ├── QuizGame.Infrastructure/  # EF Core, GBFS clients, JWT, background sync
│   └── QuizGame.Api/             # Minimal API endpoints, static frontend
└── tests/
    └── QuizGame.Tests/           # Domain unit tests
```

---

## Architecture

Clean Architecture with four layers. Dependencies point inward only:

```
QuizGame.Api  →  QuizGame.Application  →  QuizGame.Domain
                          ↑
              QuizGame.Infrastructure
```

**Domain** has zero NuGet dependencies. It holds the entities (`GameSession`, `User`, `Attempt`), the `QuestionGenerator` domain service, and the scoring constants. All business rules live here — the 60-second session timer, the +50/−20 scoring, the Won/Lost outcome.

**Application** defines the interfaces (`IGameSessionRepository`, `IGbfsClient`, `IStationCache`) and implements the use cases (`AuthService`, `GameService`). It knows what needs to happen but not how — that's Infrastructure's job.

**Infrastructure** implements everything that touches the outside world: EF Core repositories, three GBFS HTTP clients, JWT token generation, and a background sync service that polls the providers every 60 seconds using `PeriodicTimer`.

**API** is thin. `Program.cs` wires up DI and maps six HTTP endpoints. Each endpoint parses the request, calls an Application service, and returns the result. No business logic.

---

## Tech choices

**ASP.NET Core 10 minimal APIs** — no controller boilerplate for six endpoints. The route definitions are readable and the DI integration is clean.

**EF Core + PostgreSQL** — relational model fits well here. Sessions own attempts, users own sessions. EF migrations give a reproducible schema without writing SQL by hand.

**`IMemoryCache` over Redis** — the GBFS station data is refreshed every 60 seconds and lives in memory. For a single-instance deployment `IMemoryCache` is sufficient and removes an operational dependency. The abstraction (`IStationCache`) means swapping to Redis later is one class change.

**`IHostedService` over Hangfire** — the GBFS sync is a simple periodic loop. `PeriodicTimer` in a `BackgroundService` handles it in ~20 lines. Hangfire would add a UI, persistence, and retry logic that this use case doesn't need.

**Vanilla JS frontend** — a single `index.html` served as a static file. No build step, no Node, no bundler. For a game with five screens and a few API calls, a framework would add complexity without adding value. The file ships with the API binary.

**JWT bearer auth** — stateless, no session storage needed. Tokens are issued on login and validated on every request. `JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear()` is called at startup to prevent ASP.NET Core from remapping the `sub` claim.

---

## GBFS providers

| Provider | City | Discovery URL |
|---|---|---|
| Oslo Bysykkel | Oslo 🇳🇴 | `gbfs.urbansharing.com/oslobysykkel.no` |
| Citi Bike | New York 🇺🇸 | `gbfs.citibikenyc.com/gbfs/2.3` |
| Cyclocity | Paris 🇫🇷 | `api.cyclocity.fr/contracts/paris` |

Provider URLs are in `appsettings.json` under `Gbfs`. Adding a fourth provider is one class and two config lines.

---

## Game flow

```
POST /auth/register   →  create account, receive JWT
POST /auth/login      →  receive JWT

POST /games           →  start session, receive first question + ExpiresAt
POST /games/{id}/answer  →  submit answer, receive result + next question
GET  /games/history   →  all past sessions for the current user
```

The 60-second timer is enforced server-side via `GameSession.ExpiresAt`. The client displays a countdown but the server validates the deadline on every answer submission.

---

## What I would change with more time

- **Distributed session queue** — the in-memory `Dictionary<Guid, Queue<Question>>` works for a single instance but breaks under load balancing or restarts. Moving it to Redis with a short TTL would make it production-safe.
- **Integration tests** — domain logic is unit tested. The HTTP layer and database interaction are not. `WebApplicationFactory` + Testcontainers for PostgreSQL would cover the full stack without external dependencies.
- **Rate limiting** — the answer endpoint has no throttle. A determined client could submit answers faster than humanly possible. ASP.NET Core's built-in rate limiting middleware would handle this in a few lines.
- **Typed result instead of exceptions** — Application services currently throw exceptions for control flow (e.g. `UnauthorizedAccessException`, `KeyNotFoundException`). A `Result<T>` type would make error handling explicit and remove the try/catch blocks from endpoints.
