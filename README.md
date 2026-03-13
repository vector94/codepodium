# CodePodium

A programming contest aggregator that pulls contests from Codeforces and LeetCode, with search and real-time statistics.

## Features

- Browse and filter contests by platform (Codeforces, LeetCode)
- Search contests by name across all platforms
- Paginated contest listing with status indicators (Upcoming, Ongoing, Finished)
- Sync contests on demand from external APIs
- Stats overview: total contests, upcoming, ongoing, finished, and per-platform breakdown

## Tech Stack

**Backend**
- .NET 8 / ASP.NET Core Web API
- Entity Framework Core + PostgreSQL (production), SQLite (tests)
- Clean Architecture (Core, Infrastructure, API layers)

**Frontend**
- React + TypeScript + Vite
- Tailwind CSS

**CI/CD**
- GitHub Actions — runs on every push and pull request
- xUnit for unit and integration tests

## Project Structure

```
.
├── src/
│   ├── CodePodium.Core/          # Domain models, interfaces, services
│   ├── CodePodium.Infrastructure/ # EF Core, repositories, external API fetchers
│   └── CodePodium.API/           # ASP.NET Core Web API
├── tests/
│   ├── CodePodium.UnitTests/     # Unit tests with NSubstitute mocks
│   └── CodePodium.IntegrationTests/ # Integration tests with WebApplicationFactory
└── frontend/                     # React frontend
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 18+
- PostgreSQL (for local development)

### Backend

1. Create a local PostgreSQL database and set the connection string in `src/CodePodium.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=codepodium;Username=<your-user>;Trust Server Certificate=true"
  }
}
```

2. Run the API:

```bash
dotnet run --project src/CodePodium.API
```

The API will be available at `http://localhost:5217`.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173` and proxies API calls to `localhost:5217`.

## Running Tests

```bash
# Unit tests
dotnet test tests/CodePodium.UnitTests

# Integration tests
dotnet test tests/CodePodium.IntegrationTests
```

## CI Pipeline

GitHub Actions runs on every push:

1. Restore dependencies
2. Build (Release configuration)
3. Run unit tests
4. Run integration tests

See [.github/workflows/ci.yml](.github/workflows/ci.yml) for the full pipeline definition.

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/contests` | List contests (supports `platform`, `search`, `page`, `pageSize`) |
| GET | `/api/contests/upcoming` | List upcoming contests |
| GET | `/api/contests/stats` | Contest statistics (total, upcoming, ongoing, finished, byPlatform) |
| GET | `/api/contests/{id}` | Get a single contest by ID |
| POST | `/api/contests/sync` | Sync contests from external APIs |
