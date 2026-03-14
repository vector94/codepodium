# CodePodium

A programming contest aggregator that pulls contests from Codeforces and LeetCode, with search and real time statistics.

> **Live Demo:** [https://codepodium-sgk3.onrender.com](https://codepodium-sgk3.onrender.com)
>
> **API Docs (Scalar):** [https://codepodium.onrender.com/scalar/v1](https://codepodium.onrender.com/scalar/v1)
>
> **Note:** Hosted on Render free tier. If the app has not been used recently, the first load may take up to 1 or 2 minutes due to a cold start.

## Features

- Browse and filter contests by platform (Codeforces, LeetCode)
- Search contests by name
- View contests with status indicators (Upcoming, Ongoing, Finished)
- Sync contests on demand from external APIs
- Stats overview showing total, upcoming, ongoing and finished contests with a per platform breakdown

## Tech Stack

**Backend**
- .NET 8 and ASP.NET Core Web API
- Entity Framework Core with PostgreSQL (production) and SQLite (tests)
- Clean Architecture with Core, Infrastructure and API layers

**Frontend**
- React with TypeScript and Vite
- Tailwind CSS

**Version Control**
- Git with GitHub as the remote repository

**CI/CD**
- GitHub Actions runs on every push and pull request
- Dotnet build tool compiles the project in Release configuration
- xUnit for unit and integration tests running automatically on every push
- NSubstitute for mocking dependencies in unit tests
- Render deploys the latest version automatically on every push to main

## Project Structure

```
.
├── src/
│   ├── CodePodium.Core/           # Domain models, interfaces, services
│   ├── CodePodium.Infrastructure/ # EF Core, repositories, external API fetchers
│   └── CodePodium.API/            # ASP.NET Core Web API
├── tests/
│   ├── CodePodium.UnitTests/      # Unit tests with NSubstitute mocks
│   └── CodePodium.IntegrationTests/ # Integration tests with WebApplicationFactory
└── frontend/                      # React frontend
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 18 or higher
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
2. Build in Release configuration
3. Run unit tests
4. Run integration tests
5. Render automatically deploys the latest version on every push to main

See [.github/workflows/ci.yml](.github/workflows/ci.yml) for the full pipeline definition.

## API Endpoints

Production base URL: `https://codepodium.onrender.com`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/contests` | List contests (supports platform, search, page, pageSize) |
| GET | `/api/contests/upcoming` | List upcoming contests |
| GET | `/api/contests/stats` | Contest statistics (total, upcoming, ongoing, finished, by platform) |
| GET | `/api/contests/{id}` | Get a single contest by ID |
| POST | `/api/contests/sync` | Sync contests from external APIs |
