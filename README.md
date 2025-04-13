# Discount Platform

This is a discount platform project with API, MVC, and a background worker service.

The platform allows companies to post discounted offers (especially for perishable items), and customers to purchase them at the end of the day.

 🧑‍🤝‍🧑 Roles

There are three main roles, each with specific permissions:

- Admin
  - Create categories (e.g., Food, Plants, Drinks, etc.)
  - Activate registered companies
  - View lists of companies, customers, offers, and purchases

- Company
  - Create offers with price, quantity, category, and expiration time
  - Cancel an offer within 10 minutes (customers are refunded automatically)
  - Upload images for their profile and offers
  - View both current and archived offers

- Customer
  - Subscribe to categories and get relevant offers
  - Purchase products (their balance and offer quantity are updated)
  - Cancel a purchase within 5 minutes (refunds balance and restores quantity)

> ⚠️ Guests (unauthenticated users) are automatically redirected to the homepage.

 🛠 Features

- Offers expire and are automatically moved to the archive by a background worker
- Local file storage is used for uploading images
- Role-based access and authorization using custom JWT authentication
- MVC and Swagger interfaces both use the core application layer
- Pagination support for listings 

📚 Tech Stack & Architecture

- SQL Server
- .NET Core Web API, MVC, Background Worker
- Entity Framework Core (Code First, Seeding, Migrations)
- Clean Architecture
- Repository & Unit of Work patterns
- API Versioning
- Swagger + XML Comments
- Global Exception Handling Middleware
- FluentValidation
- Mapster
- JWT Authentication
- Asynchronous Programming with `CancellationToken`
- Health Checks
- Client-side and Server-side Validation
- Unit Testing for the Application Layer
- Follows OOP and SOLID principles

## 🚀 Getting Started

To run this project locally:

1. Clone the repository:
   git clone https://github.com/irakli1610/DiscountPlatform.git
   
2. Modify the connection string in appsettings.json to point to your SQL Server.

3. Adjust ImageUploadSettings in the config to set your desired image storage path.

4. Apply migrations and run the solution.
