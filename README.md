# Customer Account Management

ASP.NET Core MVC web application for managing client invoices.

## Tech Stack

- .NET 9, ASP.NET Core MVC
- Entity Framework Core 9
- Microsoft SQL Server / LocalDB
- Bootstrap 5 + Bootstrap Icons

## Features

- **Clients** — create and list clients with unique email enforcement
- **Invoices** — create, list, and delete invoices
- **Filtering** — filter invoice list by client and/or currency
- **PDF upload** — attach a PDF document to each invoice
- **Validation** — server-side and client-side validation for all forms

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- SQL Server or SQL Server LocalDB (ships with Visual Studio)

### 1. Clone the repository

```bash
git clone https://github.com/Hauruz/customer-account-management.git
cd customer-account-management
```

### 2. Configure the database connection

Open `CustomerAccountManagement/appsettings.json` and set your connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CustomerAccountManagement;Trusted_Connection=True;"
}
```

### 3. Apply EF Core migrations

```bash
cd CustomerAccountManagement
dotnet ef database update
```

This will create the database and all tables automatically.

**Alternatively**, run the SQL script manually in SSMS or Azure Data Studio:

```
scripts/schema.sql
```

### 4. Run the application

```bash
dotnet run
```

Open https://localhost:5001 (or http://localhost:5000) in your browser.

The app starts on the **Invoices** page by default.

## PDF Storage

Uploaded PDF files are saved to `CustomerAccountManagement/wwwroot/uploads/`.  
Each file is stored with a GUID-based name to avoid collisions.  
The original filename is preserved in the database and shown in the UI.

## Project Structure

```
CustomerAccountManagement/
├── Controllers/          — ClientsController, InvoicesController
├── Data/                 — ApplicationDbContext (EF Core)
├── Enums/                — Currency enum
├── Models/               — Client, Invoice domain entities
├── ViewModels/           — Strongly-typed view models
│   ├── Clients/
│   └── Invoices/
├── Views/                — Razor views
│   ├── Clients/
│   ├── Invoices/
│   └── Shared/
└── wwwroot/uploads/      — PDF file storage
scripts/
└── schema.sql            — SQL script for manual DB creation
```

## Database Script

See [`scripts/schema.sql`](scripts/schema.sql) for a ready-to-run schema creation script.
