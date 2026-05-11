# GardenApi

A RESTful API for managing gardens, garden items (containers/beds), and plants. Built with .NET 7, Entity Framework Core, and SQL Server. Includes JWT authentication and ~780 pre-seeded plants across 9 categories.

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (required for the Docker path)
- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7) (required only for local development)

---

## Running with Docker (Recommended)

This is the easiest way to run the full application — no local .NET or SQL Server installation needed.

**1. Clone the repository**

```bash
git clone <repository-url>
cd GardenApi
```

**2. Start the application**

```bash
docker compose up --build
```

Docker will:
- Pull and start a SQL Server 2022 Express container
- Build and start the API container
- Create the database schema automatically on first boot
- Seed ~780 plants across all categories

**3. Access the API**

| Resource | URL |
|---|---|
| Swagger UI | http://localhost:8080/swagger |
| API Base | http://localhost:8080/api |
| SQL Server | `localhost:1433` (sa / GardenApi!Str0ng) |

**4. Stop the application**

```bash
docker compose down
```

To also remove the database volume (full reset):

```bash
docker compose down -v
```

> **Note:** After any code change, rebuild the image with `docker compose up --build` to apply the changes.

---

## Running Locally (without Docker)

You will need SQL Server (LocalDB is included with Visual Studio) and the .NET 7 SDK.

**1. Restore dependencies and build**

```bash
dotnet restore
dotnet build
```

**2. Update the connection string** (optional — the default targets LocalDB)

The default connection string in [src/GardenApi.API/appsettings.json](src/GardenApi.API/appsettings.json) points to SQL Server LocalDB:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GardenDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

If you are using a full SQL Server instance, update the connection string accordingly.

**3. Run the API**

```bash
cd src/GardenApi.API
dotnet run
```

The API will be available at:
- Swagger UI: http://localhost:5080/swagger
- API Base: http://localhost:5080/api

The database schema and seed data are applied automatically on first startup.

---

## Authentication

All endpoints except `GET /api/plants*` and the two auth endpoints require a JWT bearer token.

**1. Register a new user**

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "alice",
  "email": "alice@example.com",
  "password": "Password123!"
}
```

**2. Login and get a token**

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "alice@example.com",
  "password": "Password123!"
}
```

The response contains a `token` field. Pass it as a header on all subsequent requests:

```http
Authorization: Bearer <token>
```

In Swagger UI, click **Authorize** at the top right and enter `Bearer <token>`.

---

## API Reference

### Auth

#### `POST /api/auth/register`
Register a new user account. Returns a JWT token that can be used immediately.

**Request body:**
```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

**Responses:**
- `201` — Registration successful, returns `AuthResponse` with a JWT token
- `400` — Validation error
- `409` — Email already in use

---

#### `POST /api/auth/login`
Authenticate an existing user and receive a JWT bearer token.

**Request body:**
```json
{
  "email": "string",
  "password": "string"
}
```

**Responses:**
- `200` — Login successful, returns `AuthResponse` with a JWT token
- `401` — Invalid email or password

---

### Plants

All plant read endpoints are public (no token required). Create, update, and delete require authentication.

#### `GET /api/plants`
Returns all plants in the database.

**Response:** Array of `PlantDto`

---

#### `GET /api/plants/{id}`
Returns a single plant by ID.

**Response:** `PlantDto` or `404`

---

#### `GET /api/plants/by-type/{type}`
Returns all plants matching the given category.

**`type` values:** `Vegetable`, `Fruit`, `Herb`, `Flower`, `Tree`, `Shrub`, `Grass`, `Succulent`, `Vine`

**Response:** Array of `PlantDto`

---

#### `GET /api/plants/by-zone/{zone}`
Returns all plants whose planting zone range contains the given USDA zone number.

For example, zone `8` will match plants with ranges like `6-10` or `7-9`.

**`zone`:** Integer (typically 1–13)

**Response:** Array of `PlantDto`

---

#### `GET /api/plants/by-sunlight/{sunlightLevel}`
Returns all plants that match the given sunlight requirement.

**`sunlightLevel` values:** `FullSun`, `PartialShade`, `FullShade`

**Response:** Array of `PlantDto`

---

#### `POST /api/plants`
Creates a new plant. Requires authentication.

**Request body:**
```json
{
  "name": "string",
  "type": "Vegetable",
  "sunlightRequirement": "FullSun",
  "waterFrequencyDays": 3,
  "spacing": 12.0,
  "plantingZone": "5-9",
  "description": "string"
}
```

> `spacing` is in inches. `waterFrequencyDays` is how often (in days) the plant needs watering — e.g., `3` means water every 3 days.

**Response:** `201` with the created `PlantDto`

---

#### `PUT /api/plants/{id}`
Updates an existing plant. Requires authentication.

**Request body:** Same fields as `POST /api/plants`

**Responses:** `200` with updated `PlantDto`, or `404`

---

#### `DELETE /api/plants/{id}`
Deletes a plant. Requires authentication.

**Responses:** `204`, or `404`

---

### Gardens

All garden endpoints require authentication.

#### `GET /api/gardens`
Returns all gardens with a count of their garden items.

**Response:** Array of `GardenDto`

---

#### `GET /api/gardens/{id}`
Returns a single garden by ID.

**Response:** `GardenDto` or `404`

---

#### `POST /api/gardens`
Creates a new garden.

**Request body:**
```json
{
  "name": "string",
  "description": "string"
}
```

**Response:** `201` with the created `GardenDto`

---

#### `PUT /api/gardens/{id}`
Updates a garden's name or description. Requires authentication.

**Request body:** Same fields as `POST /api/gardens`

**Responses:** `200` with updated `GardenDto`, or `404`

---

#### `DELETE /api/gardens/{id}`
Deletes a garden and all of its garden items (cascade delete). Requires authentication.

**Responses:** `204`, or `404`

---

### Garden Items

Garden items represent physical containers or planting beds within a garden. Each item tracks its dimensions, sunlight, and which plants have been assigned to it. Available area is calculated automatically as plants are added or removed.

All garden item endpoints require authentication.

#### `GET /api/gardenitems`
Returns all garden items with their assigned plants and area data.

**Response:** Array of `GardenItemDto`

---

#### `GET /api/gardenitems/{id}`
Returns a single garden item by ID, including its full plant list and area calculations.

**Response:** `GardenItemDto` or `404`

---

#### `GET /api/gardenitems/by-garden/{gardenId}`
Returns all garden items belonging to a specific garden.

**Response:** Array of `GardenItemDto`

---

#### `GET /api/gardenitems/{id}/plant-capacity/{plantId}`
Calculates how many of a given plant can fit in a container, taking into account plants already assigned.

Returns:
- `TotalContainerCapacity` — maximum number that fit in the full container (grid packing)
- `UsedArea` — area already consumed by assigned plants (in square inches)
- `AvailableArea` — remaining open area
- `CountInAvailableSpace` — how many more of this plant fit in the remaining area

**Responses:** `200` with `PlantCapacityDto`, or `404` if the item or plant is not found

---

#### `GET /api/gardenitems/{id}/remaining-space`
Returns all plants that can physically fit into the remaining available space of a container. Results are ordered by how many of each plant could fit (most to least).

**Response:** Array of `RemainingSpacePlantDto` (each item has a `plant` and a `canFit` count), or `404`

---

#### `POST /api/gardenitems`
Creates a new garden item within an existing garden.

**Request body:**
```json
{
  "gardenId": 1,
  "nickname": "Front Raised Bed",
  "width": 48.0,
  "height": 24.0,
  "type": "RaisedBed",
  "sunlightReceived": "FullSun"
}
```

> `nickname` is optional. `width` and `height` are in inches. `type` values: `RaisedBed`, `Container`, `InGround`, `WindowBox`. `sunlightReceived` values: `FullSun`, `PartialShade`, `FullShade`.

**Responses:** `201` with the created `GardenItemDto`, or `404` if the garden is not found

---

#### `PUT /api/gardenitems/{id}`
Updates a garden item's dimensions, type, sunlight, or nickname. Does not affect assigned plants.

**Request body:** Same fields as `POST /api/gardenitems` (excluding `gardenId`)

**Responses:** `200` with updated `GardenItemDto`, or `404`

---

#### `DELETE /api/gardenitems/{id}`
Deletes a garden item and all of its plant assignments.

**Responses:** `204`, or `404`

---

#### `PUT /api/gardenitems/{id}/plant/{plantId}`
Assigns a plant to a garden item. Before adding the plant, the API checks:
1. The plant's sunlight requirement matches the container's sunlight level.
2. The plant's spacing fits within the container's dimensions.
3. The plant's spacing area (`spacing × spacing` sq inches) fits within the remaining available area after all previously assigned plants are accounted for.

Returns `422` with a descriptive error message if any check fails.

**Responses:** `200` with updated `GardenItemDto`, `404` if the item or plant is not found, or `422` if there is insufficient space or a sunlight mismatch

---

#### `DELETE /api/gardenitems/{id}/plant/{plantId}`
Removes a specific plant assignment from a garden item. The available area is recalculated automatically after removal.

**Responses:** `200` with updated `GardenItemDto`, or `404` if the item or plant assignment is not found

---

## Plant Categories

The database is seeded with plants in these categories:

| Category | Count |
|---|---|
| Vegetable | 100 |
| Fruit | 100 |
| Flower | 100 |
| Tree | 100 |
| Shrub | 100 |
| Herb | 70 |
| Vine | 60 |
| Succulent | 100 |
| Grass | 50 |

---

## Space Tracking

Available area on each garden item is tracked dynamically:

- `TotalArea` = `Width × Height` (in square inches)
- Each plant occupies `Spacing × Spacing` square inches
- `UsedArea` = sum of `Spacing²` for all assigned plants
- `AvailableArea` = `TotalArea − UsedArea`

When assigning a new plant, the API validates that `plant.Spacing²` fits within `AvailableArea`. When a plant is removed, its spacing area is returned to the available pool.

---

## Project Structure

```
GardenApi/
├── src/
│   ├── GardenApi.API/           # Controllers, Swagger, JWT setup, startup
│   ├── GardenApi.Application/   # Services, DTOs, interfaces, business logic
│   ├── GardenApi.Infrastructure/# EF Core, repositories, database initializer
│   └── GardenApi.Domain/        # Models, enums, repository interfaces
├── tests/
│   └── GardenApi.Tests/         # xUnit unit tests
├── seed_plants.sql              # Standalone SQL seed script (for manual use)
├── Dockerfile
└── docker-compose.yml
```

---

## Running Tests

```bash
dotnet test
```

---

## Manual Database Seeding

If you want to seed the database manually (e.g., against an existing instance), run the included SQL script:

```bash
sqlcmd -S <server> -d GardenDb -U <user> -P <password> -i seed_plants.sql
```

Or open `seed_plants.sql` in SSMS or Azure Data Studio and execute it.
