
A simple multi-company helpdesk and ticketing system built with ASP.NET Core MVC, C#, Entity Framework Core, SQL Server and ASP.NET Core Identity.

## Technologies

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- HTML, CSS, JavaScript, jQuery
- Bootstrap

## Main Features

- Login and registration
- Role-based access: CompanyAdmin, Agent, Customer
- Multi-company tenant isolation
- Ticket creation and editing
- Ticket assignment
- Ticket status workflow
- Comments and ticket history
- SLA due dates and overdue tickets
- Notifications with AJAX polling
- Dashboard
- Optimistic concurrency using RowVersion

## Setup

1. Clone the repository.
2. Open the project in Visual Studio.
3. Update the SQL Server connection string in `appsettings.json` if required.
4. Run the project.
5. The application seeds the database with sample companies, users, roles and tickets.

Connection String:
"DefaultConnection": "DESKTOP-C4SH7TO\\SQLEXPRESS;Database=HelpdeskSystemDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"


## Seeded Login Credentials

### CompanyA

CompanyAdmin - adminA@gmail.com - Admin@123
Agent - agentA@gmail.com - Agent@123
Customer - customerA@gmail.com - Customer@123 

### CompanyB


CompanyAdmin - adminB@gmail.com - Admin@123 
Agent - agentB@gmail.com - Agent@123
Customer - customerB@gmail.com - Customer@123 

## Project Structure

HelpdeskSystem/
├── Controllers/
├── Data/
├── Models/
├── Repositories/
├── Services/
├── ViewModels/
├── Views/
├── wwwroot/
│   ├── css/
│   └── js/
├── Migrations/
├── Program.cs
└── appsettings.json


The application follows:

Controllers → Services → Repositories → DbContext → SQL Server


## Tenant Isolation

Each user belongs to a company through `CompanyId`.

The logged-in user's `CompanyId` is stored in the authentication claims. Company-specific entities such as Tickets, TicketComments, TicketHistory and Notifications use EF Core global query filters based on the current company.

This prevents users from accessing another company's data by changing a ticket ID in a URL or request.

## Concurrency

Tickets contain a SQL Server `RowVersion` column.

When a ticket is edited or its status is changed, the submitted RowVersion is checked against the current database version. If another user has already modified the ticket, the second operation is rejected with a message asking the user to refresh the page.

## Ticket Workflow

New → Assigned → In Progress → Resolved → Closed

A Resolved ticket can move back to In Progress. A Closed ticket cannot be reopened.

## Notifications

Notifications are created for ticket assignment, status changes and comments. The notification count is refreshed using jQuery AJAX polling every 30 seconds.

## Demo

Use the seeded Amazon and Flipkart accounts to demonstrate:

1. Separate company data.
2. Ticket workflow and assignment.
3. Ticket history and notifications.
4. Concurrency using the same ticket in two browser windows.
