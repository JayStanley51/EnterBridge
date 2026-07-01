EnterBridge POC
===============

A proof-of-concept ASP.NET Core MVC application that consumes the EnterBridge Case Study
Pricing API to browse products, manage pricing data, and submit purchase orders.

External API
------------
Base URL: https://api.casestudy.enterbridge.com
Swagger:  https://api.casestudy.enterbridge.com/swagger/index.html

Requirements
------------
- .NET 10 SDK
- Internet access (to reach the external API)

Running the Application
-----------------------
1. Open a terminal in the project directory.
2. Run: dotnet run
3. Navigate to https://localhost:{port} in your browser.
4. The Swagger UI for the local API proxy is available at /swagger.

Login
-----
Select a role from the dropdown on the login page:
  - Foreman
  - Purchaser / Purchaser2 / Purchaser3

Each role has different permissions (see Roles section below).

Roles & Permissions
-------------------
Foreman:
  - View all orders placed by any user
  - See count of orders awaiting approval on the dashboard
  - Edit order items and quantities before approving or rejecting
  - Approve or reject pending orders
  - View approved orders in a separate collapsed section in Order History

Purchaser / Purchaser2 / Purchaser3:
  - Place new orders
  - View only their own order history
  - Reorder a previous order (loads items back into the active cart)

Features
--------
Products Page
  - Products grouped by category (collapsible sections)
  - Displays latest price and a 3-point pricing trend indicator per product
  - Expandable price history per product
  - SKU search with live filtering

Order Page
  - Products listed by category (collapsible sections)
  - SKU search with live filtering
  - Add items to an active cart (latest price is captured at time of add)
  - Running cart count displayed in the header
  - Place Order submits the cart and records it

Order History
  - Foreman sees all orders; other roles see only their own
  - Orders show date, placed-by (Foreman only), item count, and approval status
  - Foreman: Edit / Approve / Reject buttons on pending orders
  - Foreman: Approved orders collapsed into a separate section
  - Non-Foreman: Reorder button to reload a past order into the cart

Order Storage
-------------
Orders are held in memory during the application session and persisted to:

  Data/orders.csv

The CSV is read on startup so orders survive application restarts.

CSV columns:
  OrderId, PlacedAt, Role, ProductId, Name, SKU, Quantity,
  PriceAtOrder, UnitOfMeasure, Status, ReviewedAt

Project Structure
-----------------
Controllers/
  HomeController.cs          Login, Dashboard
  ProductsController.cs      Products listing (MVC)
  OrderController.cs         Order flow, history, approval
  Api/                       API proxy controllers (Swagger-visible)

Models/
  Api/                       DTOs and enums matching the external API
  Order.cs                   Order model with approval status
  OrderItem.cs               Line item with price snapshot
  ProductWithPricesViewModel.cs
  EditOrderViewModel.cs

Services/
  IEnterBridgeApiService.cs  HTTP client interface
  EnterBridgeApiService.cs   Typed HttpClient implementation
  IOrderStore.cs             In-memory + CSV order store interface
  OrderStore.cs              Singleton implementation

Views/
  Home/    Index (login), Dashboard
  Products/Index
  Order/   Index, Confirmation, History, EditOrder, _OrderRow (partial)
