# MiniSalesMVC

MiniSalesMVC is an ASP.NET Core MVC application that simulates a sales order workflow using SQL Server and Entity Framework Core.

It includes customer management, product management, sales orders, shipments, invoices, and a dashboard with status tracking.

---

## Overview

This project allows you to:

- Manage customers
- Manage products
- Create sales orders
- Add and remove items from orders
- Process orders through their lifecycle
- Register shipments
- Generate invoices
- Mark invoices as paid
- View dashboard totals and order statuses

---

## Tech Stack

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Razor Views
- Bootstrap

---

## Order Flow

Orders move through this sequence:

```text
New -> Processing -> Shipped -> Invoiced
Rules implemented
Every order starts as New
Items can only be added while the order is New or Processing
An order must have items before it can be processed
Shipment can only be created when the order is Processing
Invoice can only be generated when the order is Shipped
Product stock is reduced when the order is shipped
Totals are recalculated automatically when items are added or removed
Prerequisites

Make sure you have installed:

.NET SDK 10
SQL Server
SQL Server Management Studio (recommended)
Entity Framework Core CLI tools

Check your .NET version:

dotnet --version

Check EF Core CLI tools:

dotnet ef

If dotnet-ef is not installed globally:

dotnet tool install --global dotnet-ef

If it is already installed and you want to update it:

dotnet tool update --global dotnet-ef
Clone the Repository
git clone https://github.com/CarlosPuent/MiniSalesMVC.git
cd MiniSalesMVC
Database Setup

This project uses a SQL Server database named:

RsmMiniSalesOrderDb

Create the database in SQL Server:

CREATE DATABASE RsmMiniSalesOrderDb;
GO
Configuration

Update the SQL Server connection string in:

Rsm.MiniSalesOrder.Web/appsettings.Development.json

Use your own SQL Server instance, username, and password.

Example:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_INSTANCE;Database=RsmMiniSalesOrderDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}

Example for a local instance:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=LapPUENTE\\PUENT;Database=RsmMiniSalesOrderDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  }
}

Replace the following values:

YOUR_SERVER_INSTANCE
YOUR_USERNAME
YOUR_PASSWORD
Optional

You can leave appsettings.json with an empty connection string and keep real local credentials only in appsettings.Development.json.

Example:

{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
Restore and Build

Restore packages:

dotnet restore

Build the solution:

dotnet build

If the build succeeds, continue with migrations.

Apply Migrations

Create the migration:

dotnet ef migrations add InitialCreate --project .\Rsm.MiniSalesOrder.Infrastructure\ --startup-project .\Rsm.MiniSalesOrder.Web\

Apply the migration:

dotnet ef database update --project .\Rsm.MiniSalesOrder.Infrastructure\ --startup-project .\Rsm.MiniSalesOrder.Web\

This will:

Create the tables
Create relationships and indexes
Insert seed data
Seed Data

After applying migrations, the database includes sample data for testing.

Customers
Juan Pérez
María González
Carlos Rodríguez
Products
Laptop HP
Monitor Dell
Teclado Logitech
Mouse Microsoft
Impresora Canon
Run the Application

Start the application:

dotnet run --project .\Rsm.MiniSalesOrder.Web\

Then open the local URL shown in the terminal, for example:

https://localhost:xxxx
Recommended Test Flow
1. Verify initial data
Open Customers
Open Products
2. Create a sales order
Go to Sales Orders
Click Create New Order
Select a customer
Save
3. Add items to the order
Open the order details
Click Add Item
Select a product
Enter quantity
Save
4. Process the order
Click Process Order
5. Register shipment
Click Register Shipment
Enter shipment data
Save
6. Generate invoice
Click Generate Invoice
Save
7. Mark invoice as paid
Use the button in the order details page
Project Structure
RsmMiniSalesOrder
│
├── Rsm.MiniSalesOrder.Web
│   ├── Controllers
│   ├── Models
│   ├── Views
│   └── appsettings.Development.json
│
├── Rsm.MiniSalesOrder.Application
│   └── Interfaces
│
├── Rsm.MiniSalesOrder.Domain
│   ├── Entities
│   └── Enums
│
└── Rsm.MiniSalesOrder.Infrastructure
    ├── Data
    └── Services
Troubleshooting
SQL connection error

Check that:

SQL Server is running
The server instance name is correct
The username and password are correct
TrustServerCertificate=True is included if needed
Build succeeds but database update fails

Check that:

The database exists
The connection string is correct
Migration files exist
The SQL Server login has permissions
Pending model changes warning

If this appears, make sure seed data uses fixed values instead of dynamic values such as:

DateTime.Now
Guid.NewGuid()
EF Core tools version warning

If dotnet-ef is older than the runtime, update it:

dotnet tool update --global dotnet-ef
Notes
This project is intended for learning, demo, and portfolio purposes
It uses MVC with Razor Views for simplicity and fast testing
It is designed so anyone can clone the repository, configure SQL Server, run migrations, and test the functionality
Author

Carlos Puente
