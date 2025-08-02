# Car Service Management System

A car service management application built with ASP.NET Core that allows administrators, mechanics, and customers to manage car services.

## Technologies Used

- **Backend**: ASP.NET Core 7.0
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Frontend**: ASP.NET Core Razor Pages with Bootstrap
- **Testing**: xUnit
- **API**: ASP.NET Core Web API

## Prerequisites

- .NET 7.0 SDK
- PostgreSQL Database
- Visual Studio 2022 or VS Code

## Database Setup

1. Install PostgreSQL and create a database named `CarServ`
2. Update the connection string in `appsettings.json` if needed:
   ```
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=CarServ;Username=postgres;Password=usman"
   }
   ```

## Running the Application

### 1. Database Migrations

Navigate to the Domain project and run migrations:
```
cd CarServPro.Domain
dotnet ef database update
```

### 2. Run the Razor Application

Navigate to the Razor project and start the application:
```
cd CarServPro.Razor
dotnet run
```

The application will be available at `http://localhost:5149/` 

### 3. Run the API

Navigate to the API project and start the API:
```
cd CarServPro.Api
dotnet run
```

Swagger documentation will be available at `/swagger`

### 4. Run Tests

Navigate to the Test project and run tests:
```
cd CarServPro.Test
dotnet test
```

## Default Users

The application creates default users on first run:

### Admin
- Email: `admin@carservice.com`
- Password: `Dorset001^`

### Mechanics
- Email: `mechanic1@carservice.com`
- Password: `Dorset001^`
- Email: `mechanic2@carservice.com`
- Password: `Dorset001^`

### Customers
- Email: `customer1@carservice.com`
- Password: `Dorset001^`
- Email: `customer2@carservice.com`
- Password: `Dorset001^`

## Features

### Admin Dashboard
- View all customers
- Add new customers
- Delete customers
- View customer details and cars
- Add services to cars

### Mechanic Dashboard
- View assigned services
- Complete services with work description and hours
- Automatic cost calculation

### Customer Dashboard
- View their cars and service history
- See service costs and completion dates

## API Endpoints

- `GET /api/CarAndService/customers` - Get all customers
- `GET /api/CarAndService/mechanics` - Get all mechanics
- `GET /api/CarAndService/cars` - Get all cars with customer info
- `GET /api/CarAndService` - Health check


## Project Structure

- `CarServPro.Domain` - Entity models and DbContext
- `CarServPro.Razor` - Main web application
- `CarServPro.Api` - REST API
- `CarServPro.Test` - Unit tests 