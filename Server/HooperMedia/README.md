# HooperMedia Server

This folder contains the backend API, SQL Server database, infrastructure, and tests for the HooperMedia application.

## Services

The Docker Compose setup starts:

- `sqlserver`: Microsoft SQL Server 2022
- `api`: ASP.NET Core API (`HooperMedia.Api`)

## Docker

Run commands from the server root:

```bash
Server/HooperMedia
```

Build and start all backend services:

```bash
docker compose up --build
```

Run in detached mode:

```bash
docker compose up --build -d
```

Stop and remove containers and network:

```bash
docker compose down
```

If you need a full clean rebuild:

```bash
docker compose down --remove-orphans
docker compose build --no-cache
docker compose up -d --force-recreate
```

## Ports

- API: `http://localhost:8080`
- SQL Server: `localhost:1433`

## Database

The API connects to SQL Server using the connection string defined in [docker-compose.yml](docker-compose.yml).

Default SQL Server credentials used by Docker Compose:

- Username: `sa`
- Password: `YourStrong!Passw0rd`
- Database: `HooperMediaDb`

Entity Framework Core migrations are applied automatically when the API starts.

## API Endpoints

Common endpoints:

- `GET /Status`
- `GET /Person`
- `GET /Person/{personId}`
- `POST /Person`
- `PUT /Person/{personId}`
- `DELETE /Person/{personId}`
- `GET /Address`
- `GET /Address/{addressId}`
- `GET /Address/person/{personId}`
- `POST /Address`
- `PUT /Address/{addressId}`
- `DELETE /Address/{addressId}`

Example URLs:

- `http://localhost:8080/Status`
- `http://localhost:8080/Person`
- `http://localhost:8080/Address`

In development, OpenAPI is also exposed by the API.

## Project Structure

- `HooperMedia.Api`: ASP.NET Core Web API
- `HooperMedia.Core`: domain entities, validation, and business rules
- `HooperMedia.Infrastructure`: EF Core data access, repositories, and services
- `HooperMedia.Tests`: automated tests
- `Postman-Collections`: API collection for manual testing