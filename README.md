# 🎓 University Portal

A comprehensive course management system built with ASP.NET Core MVC, featuring role-based authentication, course enrolment, and administrative controls.

## 🌟 Features

### For Students
- **User Registration & Authentication** - Secure account creation and login
- **Student Profile Management** - Create and manage personal academic profile
- **Course Browsing** - Search, filter, and sort available courses
- **Course Enrolment** - Enrol in courses with real-time capacity tracking
- **Enrolment Dashboard** - View current enrolments, credits, and academic history
- **Course Withdrawal** - Drop courses when needed

### For Administrators
- **Admin Dashboard** - Overview of system statistics and recent activity
- **Course Management** - Full CRUD operations for courses
- **Student Monitoring** - View enrolled students and enrolment history
- **Capacity Management** - Track and manage course availability

## 🛠️ Technology Stack

- **Framework:** ASP.NET Core 8.0 MVC
- **Database:** SQLite with Entity Framework Core
- **Authentication:** ASP.NET Core Identity
- **Frontend:** Bootstrap 5, Razor Views
- **Language:** C# 12

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A code editor (Visual Studio, VS Code, or Rider)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/kebeattie/university-portal.git
cd university-portal
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Apply Database Migrations

```bash
dotnet ef database update
```

This will create the SQLite database with all necessary tables and seed initial data.

### 4. Run the Application

```bash
dotnet run
```

The application will start at `http://localhost:5006` (or check the console output for the exact URL).

## 👤 Default Accounts

### Administrator Account
- **Email:** admin@university.com
- **Password:** Admin@123

### Student Account
Register a new account and create your student profile to access student features.

## 📚 Sample Data

The application comes pre-seeded with 5 sample courses:
- **CS101** - Introduction to Computer Science (3 credits)
- **CS201** - Data Structures and Algorithms (4 credits)
- **MATH101** - Calculus I (4 credits)
- **ENG101** - English Composition (3 credits)
- **PHYS101** - Physics I (4 credits)

## 🗂️ Project Structure

```
university-portal/
├── Controllers/          # MVC Controllers
│   ├── AdminController.cs
│   ├── CoursesController.cs
│   ├── EnrollmentsController.cs
│   └── HomeController.cs
├── Models/              # Data models
│   ├── Course.cs
│   ├── Student.cs
│   ├── Enrolment.cs
│   └── UserRoles.cs
├── Views/               # Razor views
│   ├── Admin/
│   ├── Courses/
│   ├── Enrollments/
│   ├── Home/
│   └── Shared/
├── Data/                # Database context and migrations
│   ├── ApplicationDbContext.cs
│   ├── DbSeeder.cs
│   └── Migrations/
└── wwwroot/            # Static files (CSS, JS, images)
```

## 🔐 User Roles

### Student Role
- Browse and search courses
- Enrol in available courses
- View enrolment history
- Withdraw from courses
- Manage student profile

### Admin Role
- All student permissions
- Create, edit, and delete courses
- View all enrolments
- Access admin dashboard
- Manage course capacity

## 🎯 Key Functionalities

### Course Enrolment System
- Real-time capacity tracking
- Prevent duplicate enrolments
- Automatic enrolment count updates
- Course availability indicators

### Authentication & Authorization
- Role-based access control
- Secure password hashing
- Email-based user accounts
- Session management

### Search & Filter
- Search courses by code, name, or instructor
- Sort by course code, name, or credits
- Filter active/inactive courses

## 🗃️ Database Schema

### Main Tables
- **AspNetUsers** - User accounts (Identity)
- **AspNetRoles** - User roles (Identity)
- **Students** - Student profile information
- **Courses** - Course catalog
- **Enrolments** - Student-course relationships

### Key Relationships
- Student → User (Many-to-One)
- Enrolment → Student (Many-to-One)
- Enrolment → Course (Many-to-One)

## 🌐 Deployment

This application is designed to be deployed on platforms that support ASP.NET Core and SQLite:
- Railway
- Fly.io
- Azure App Service
- DigitalOcean App Platform

See the deployment guide for detailed instructions.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📝 License

This project is open source and available under the [MIT License](LICENSE).

## 👨‍💻 Author

**Kyle Beattie**
- GitHub: [@kebeattie](https://github.com/kebeattie)

## 🙏 Acknowledgments

- Built with ASP.NET Core
- UI components from Bootstrap
- Icons from Bootstrap Icons
- Database management with Entity Framework Core

---

**Note:** This is a portfolio project demonstrating full-stack web development capabilities with ASP.NET Core MVC.
