# Booking System

This variant persists appointments in SQL Server using Entity Framework Core. The existing Razor Pages appointment UI and booking request models are retained. Lookup lists (patients, practitioners, locations).

## Setup
1. Update `ConnectionStrings:DefaultConnection` in `appsettings.json` for your SQL Server or LocalDB.
2. Run `Database/CreateDatabase.sql` in the target database.
4. Run `dotnet restore` then `dotnet run`.

 LocalDB connection string: Based on user 

## Architecture
Razor Pages → `IAppointmentService` → EF Core DbContext → SQL Server.

Booking uses a serializable transaction and checks the shared practitioner calendar across all locations. The buffer is included in the blocked interval. Cancellation marks the row Cancelled and frees the slot.

## Features still outstanding
- Email delivery.
- Reminder service persistence/claiming.
