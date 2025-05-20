# Dawaya Online Pharmacy 🏥💊

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-6.0-purple)
![EF Core](https://img.shields.io/badge/EF_Core-6.0-green)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.0-blueviolet)
![MSSQL](https://img.shields.io/badge/MSSQL-2019-blue)


A full-featured **online pharmacy platform** built with ASP.NET Core MVC, featuring multi-role authentication, product management, and order processing.

### Frontend
| Technology | Usage |
|------------|-------|
| HTML5 | Core page structure |
| CSS3 | Styling and animations |
| JavaScript (ES6) | Client-side logic |
| jQuery | DOM manipulation & AJAX |
| Bootstrap 5 | Responsive layout & components |

### Backend
| Technology | Usage |
|------------|-------|
| ASP.NET Core MVC | Web framework |
| Entity Framework Core | ORM for MSSQL |
| LINQ | Data querying |
| C# | Business logic |
| MSSQL | Database engine |

## ✨ Features

### 👨‍💻 User Side
- ✅ Secure registration/login system
- 🛒 Interactive shopping cart with AJAX
- 💳 Checkout process with order history
- 🌓 Light/Dark mode toggle (JS/CSS)

### 👨‍⚕️ Admin Side
- 🔒 Admin dashboard with role-based auth
- 📦 CRUD operations for medicines
- 📊 Order management system

## 🛠️ Tech Stack

### Backend (My Contribution)
- **Framework**: ASP.NET Core 6 MVC
- **Database**: MSSQL with EF Core 6
- **Patterns**: 
  - Builder (for complex queries)
  - Singleton (for service classes)
  - Factory (for payment processing)
- **Tools**: LINQ, Identity Framework

### Frontend (Team Contribution)
- HTML5, CSS3, JavaScript (ES6)
- Bootstrap 5 + jQuery
- Responsive design



📂 Project Structure
Dawaya/
├── Controllers/       # MVC controllers
├── Models/            # Domain models
├── Views/             # Razor views
├── Services/          # Business logic
├── Data/              # EF Core DbContext
├── wwwroot/           # Static files
└── Migrations/        # Database migrations
