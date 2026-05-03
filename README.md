# Stock Monitoring and Analytic System (SMAS)

Final project for OOP and Database Lab — BS Data Science, 2nd Semester
University of Engineering and Technology, Lahore

We built this because honestly, most small shops in Pakistan still
track their stock in registers or basic Excel sheets. There's no
system that ties together inventory, sales, employees, and customers
in one place. SMAS is our attempt to fix that — a proper full-stack
platform that any retail business can actually use.

---

## The Team

| Name | Roll No |
|------|---------|
| Insha Amin | 2025-DS-60 |
| Umair Naeem | 2025-DS-65 |
| Muneeb Bin Anjum | 2025-DS-74 |
| Ayyan Shahid | 2025-DS-93 |

Submitted to Ms. Esha & Ms. Iram for OOP, and Ms. Amna for Database Lab.

---

## What does it actually do?

A few things we wanted to solve from day one:

- Shop owners have no idea when stock is about to run out until
  it already has. We built automatic alerts that fire before that
  happens.

- No one knows which products are actually selling and which are
  just sitting on shelves. Our sales records and reports fix that.

- Employees get reviewed based on vibes. We wanted actual numbers —
  units sold, targets hit, monthly performance — all tracked per person.

- Small shops have zero online presence. We're building an e-commerce
  front so even a neighbourhood store can take orders from anywhere
  in Pakistan and dispatch through TCS or Leopard.

- Restocking is always reactive. We're adding a forecasting engine
  that combines past sales with social media trends to predict demand
  before it spikes.

---

## Tech we're using

**Backend:** C# on ASP.NET Core 8
**Database:** PostgreSQL
**ORM:** Entity Framework Core
**Auth:** JWT tokens
**API Docs:** Swagger UI

Frontend is HTML/CSS/JavaScript — admin dashboard and a customer
storefront. Still in progress.

---

## How the code is structured

We followed a clean four-layer architecture:
Frontend (HTML/JS)
↓
ASP.NET Core Controllers
↓
C# Service Classes
↓
EF Core + PostgreSQL
Each layer only talks to the one next to it. The database never
gets touched directly from the frontend, and business logic never
lives in the controllers. Kept things clean and testable.

---

## Database tables

We have 10 models, each mapping to a PostgreSQL table:

| Table | What it stores |
|-------|----------------|
| Products | Everything about a product — price, stock, SKU |
| Categories | Groups products by type |
| Suppliers | Vendor info linked to products |
| Employees | Staff details, roles, monthly targets |
| Customers | Buyer profiles for the e-commerce side |
| Orders | Every order placed, with status and courier ref |
| OrderItems | Individual products inside each order |
| SalesRecords | Daily sales log per employee per product |
| StockAlerts | Triggered when stock drops below reorder level |
| ForecastRecords | Predicted demand + social trend score |

---

## OOP concepts we applied

This was also an OOP course project so we made sure the code
actually reflects what we studied:

- **Inheritance** — one base `Entity` class gives every model
  its Id and timestamps
- **Encapsulation** — all fields are private, accessed through
  properties
- **Polymorphism** — service interfaces let us swap implementations
  without touching controllers
- **Abstraction** — controllers only expose what the frontend needs,
  complex logic stays hidden in services
- **Dependency Injection** — nothing is hardcoded, services are
  injected through the built-in .NET IoC container

---

## Running it locally

You'll need .NET 8 SDK and PostgreSQL installed.

```bash
# 1. Clone the repo
git clone https://github.com/inshaamin112007-a11y/SMAS.git

# 2. Update your PostgreSQL password in appsettings.json

# 3. Restore packages
dotnet restore

# 4. Create the database tables
dotnet ef database update

# 5. Run
dotnet run
```

Swagger UI will be at `http://localhost:5033/swagger`

---

## Where we are right now

Done:
- All 10 domain models
- PostgreSQL database with full schema
- EF Core migrations applied
- JWT auth configured
- API running with Swagger

Still working on:
- API controllers for each module
- Service layer with business logic
- Frontend dashboard
- E-commerce storefront
- Demand forecasting
- Geo-analytics heat map



*UET Lahore — OOP & Database Lab, 2nd Semester 2025*
