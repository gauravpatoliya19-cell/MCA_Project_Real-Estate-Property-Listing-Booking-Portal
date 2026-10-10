# Real Estate Property Listing & Booking Portal

A web application developed with **ASP.NET Core** and **Entity Framework Core (SQLite)** designed for real estate property listing, search, inquiries, booking site visits, and management.

---

## Features

- **Property Listings & Showcase**: Browse residential, commercial, and rental properties with images, prices, details, and location maps.
- **Advanced Search & Filtering**: Filter properties by type, location, price range, bedrooms, and status.
- **Site Visit Booking & Cart**: Book schedule visits to inspect properties with automated calendar slots.
- **Admin & Management Area**: Comprehensive admin dashboard for managing properties, bookings, inquiries, and contracts.
- **Contract & Invoice Generation**: Automated PDF generation for property contracts and site visits.
- **Payment Gateway Integration**: Simulates secure online payment handling for booking fees and token amounts.
- **Localization**: Formatted for Indian currency (INR / ₹) with Lakhs & Crores denomination handling.

---

## Technology Stack

- **Framework**: ASP.NET Core 5.0
- **Language**: C#
- **Database**: SQLite with Entity Framework Core
- **Frontend**: Razor Views, Bootstrap, HTML5, CSS3, JavaScript
- **Architecture**: MVC (Model-View-Controller) with Repository Pattern & Service Layer

---

## Getting Started

### Prerequisites

- [.NET SDK 5.0 or later](https://dotnet.microsoft.com/download)
- [Visual Studio 2019/2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Running the Application

1. **Clone the repository:**
   ```bash
   git clone https://github.com/gauravpatoliya19-cell/MCA_Project_Real-Estate-Property-Listing-Booking-Portal.git
   cd MCA_Project_Real-Estate-Property-Listing-Booking-Portal
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Run the project:**
   ```bash
   dotnet run --project RealEstatePortal.Web.csproj
   ```

4. **Access the application:**
   Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`.

---

## Project Structure

```text
├── Areas/              # Admin area and management modules
├── Controllers/        # MVC Controllers handling requests
├── Data/               # Entity Framework DbContext and Seed data
├── Models/             # Domain and Database Entity models
├── Repositories/       # Data Access Repository implementations
├── Services/           # Business services (PDF, Payment Gateway)
├── ViewModels/         # View models for data transfer to views
├── Views/              # Razor Views (.cshtml)
├── wwwroot/            # Static assets (CSS, JS, Images, Libraries)
├── appsettings.json    # Application configuration and DB connection
├── Program.cs          # Application entry point and database seeding
├── Startup.cs          # Dependency injection, middleware & services
└── RealEstatePortal.sln # Visual Studio Solution file
```
