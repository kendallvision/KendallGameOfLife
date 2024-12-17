# Game of Life API

This project implements Conway's Game of Life as a RESTful API using .NET 8.0. It features persistence using SQLite, unit testing, and Docker containerization.

## Prerequisites

- .NET 8.0 SDK
- SQLite
- Docker (optional)

## Getting Started

### Local Development

1. Clone the repository
```bash
git clone https://github.com/yourusername/game-of-life.git
cd game-of-life
```

2. Build the solution
```bash
dotnet build
```

3. Run the API
```bash
cd GameOfLife.API
dotnet run
```

Alternatively, you can use the baked in yarn commands:
```bash
yarn build
yarn start
yarn test
yarn test-verbose
```

The API will be available at:
- http://localhost:5000
- https://localhost:5001
- Swagger UI: http://localhost:5000/swagger

### Docker

Alternatively, run using Docker:

```bash
docker-compose up --build
```

The containerized API will be available at:
- http://localhost:5050
- https://localhost:5051
- Swagger UI: http://localhost:5050/swagger

Note: it is supposed to create the database in the .docker-database but it is not currently working on MacOS.

## API Endpoints

### POST /api/game
Create a new board state. Example request body:
```json
{
  "state": [
    [false, true, false],
    [false, true, false],
    [false, true, false]
  ]
}
```

### GET /api/game/{id}/next
Get the next generation state for a given board ID.

### GET /api/game/{id}/steps/{count}
Get the state after a specified number of generations.

### GET /api/game/{id}/final/{maxGenerations}
Get the final stable state (if one exists within maxGenerations).

## Project Structure

- `GameOfLife.API`: Web API and controllers
- `GameOfLife.Core`: Core game logic and domain models
- `GameOfLife.Infrastructure`: Data persistence
- `GameOfLife.Tests`: Unit tests

## Running Tests

```bash
dotnet test
dotnet test --logger \"console;verbosity=detailed\"
```

## Common Patterns

### Blinker
```json
{
  "state": [
    [false, true, false],
    [false, true, false],
    [false, true, false]
  ]
}
```

### Block
```json
{
  "state": [
    [false, false, false, false],
    [false, true, true, false],
    [false, true, true, false],
    [false, false, false, false]
  ]
}
```

### Glider
```json
{
  "state": [
    [false, true, false],
    [false, false, true],
    [true, true, true]
  ]
}
```

## Development Notes

- The board automatically expands when patterns reach the edges
- SQLite database is created automatically on first run
- Tests cover basic patterns, edge cases, and error conditions
