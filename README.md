# .NET Skill Test

> Here is the test assignment.
>
> https://bitbucket.org/deby3/assignment/src/main/

---

## 🏗️ Project Architecture

```
skill-test/
├── frontend/           # React + TypeScript + Material-UI
├── backend/            # Node.js + Express + PostgreSQL
├── DotNET-service/     # .NET microservice for PDF reports
├── seed_db/            # Database schema and seed data
└── README.md           # This file
```

---

## 🚀 Quick Start

### Prerequisites

- Node.js (v16 or higher)
- PostgreSQL (v12 or higher)
- .NET SDK (v10 or higher)
- npm or yarn

### Backend Setup

```bash
cd backend
npm install
cp .env.example .env  # Configure your environment variables
npm start
```

### Frontend Setup

```bash
cd frontend
npm install
npm run dev
```

### Database Setup (if needed)

```bash
# Create PostgreSQL database
createdb school_mgmt

# Run database migrations
psql -d school_mgmt -f seed_db/tables.sql
psql -d school_mgmt -f seed_db/seed-db.sql
```

### Access the Application

| Service | URL |
|---|---|
| Frontend | http://localhost:5173 |
| Backend API | http://localhost:5007 |

**Demo Credentials**

- Email: `admin@school-admin.com`
- Password: `3OU4zn3q6Zh9`

---

## 🎯 Skill Test Problem

### Build PDF Report Generation Microservice via API Integration

- **Objective**: Create a standalone microservice in .NET to generate PDF reports for students by consuming the existing Node.js backend API.
- **Location**: `DotNET-service/` directory at the root of the project.
- **Description**: This service connects to the existing Node.js backend's `/api/v1/students/:id` endpoint to fetch student data, then uses the returned JSON to generate a downloadable PDF report.
- **Issue**: Implement missing CRUD operations for student management.
- **Skills Tested**: .NET, REST API consumption, JSON parsing, file generation, microservice integration.

### Requirements

- Create a new endpoint **`GET /api/v1/students/:id/report`** in the .NET service.
- The .NET service must **not** connect directly to the database — it must fetch data from the Node.js API.
- The developer **must** have the PostgreSQL database and the Node.js backend running to complete this task.

---

## 🛠️ Technology Stack

### Frontend

| Technology | Details |
|---|---|
| Framework | React 18 + TypeScript |
| UI Library | Material-UI (MUI) v6 |
| State Management | Redux Toolkit + RTK Query |
| Form Handling | React Hook Form + Zod validation |
| Build Tool | Vite |
| Code Quality | ESLint, Prettier, Husky |

### Backend

| Technology | Details |
|---|---|
| Runtime | Node.js |
| Framework | Express.js |
| Database | PostgreSQL |
| Authentication | JWT + CSRF protection |
| Password Hashing | Argon2 |
| Email Service | Resend API |
| Validation | Zod |

### Database

| Technology | Details |
|---|---|
| Primary DB | PostgreSQL |
| Schema | Comprehensive school management schema |
| Features | Role-based access control, leave management, notice system |

---

## 📋 Features

### Core Functionality

- **Dashboard**: User statistics, notices, birthday celebrations, leave requests
- **User Management**: Multi-role system (Admin, Student, Teacher, Custom roles)
- **Academic Management**: Classes, sections, students, class teachers
- **Leave Management**: Policy definition, request submission, approval workflow
- **Notice System**: Create, approve, and distribute notices
- **Staff Management**: Employee profiles, departments, role assignments
- **Access Control**: Granular permissions system

### Security Features

- JWT-based authentication with refresh tokens
- CSRF protection
- Role-based access control (RBAC)
- Password reset and email verification
- Secure cookie handling

---

## 🔧 Development Guidelines

### Code Standards

- **File Naming**: kebab-case for consistency across OS
- **Import Style**: Absolute imports for cleaner code
- **Code Formatting**: Prettier with consistent configuration
- **Git Hooks**: Husky for pre-commit quality checks

### Project Structure

```
frontend/src/
├── api/           # API configuration and base setup
├── assets/        # Static assets (images, styles)
├── components/    # Shared/reusable components
├── domains/       # Feature-based modules
│   ├── auth/      # Authentication module
│   ├── students/  # Student management
│   ├── notices/   # Notice system
│   └── ...
├── hooks/         # Custom React hooks
├── routes/        # Application routing
├── store/         # Redux store configuration
├── theme/         # MUI theme customization
└── utils/         # Utility functions

backend/src/
├── config/        # Database and app configuration
├── middlewares/   # Express middlewares
├── modules/       # Feature-based API modules
│   ├── auth/      # Authentication endpoints
│   ├── students/  # Student CRUD operations
│   ├── notices/   # Notice management
│   └── ...
├── routes/        # API route definitions
├── shared/        # Shared utilities and repositories
├── templates/     # Email templates
└── utils/         # Helper functions
```

---

## 🧪 Testing Instructions

1. Set up the PostgreSQL database using the files in `seed_db/`.
2. Set up and run the Node.js backend by following the [Backend Setup](#backend-setup) instructions.
3. Run the .NET service.
4. Use **curl** or Postman to make a GET request to the .NET service's `/api/v1/students/:id/report` endpoint.
5. Verify that the .NET service correctly calls the Node.js backend and that a PDF file is successfully generated.
6. Check the contents of the PDF for correctness.

---

## 📚 API Documentation

### Authentication

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/auth/login` | User login |
| `POST` | `/api/v1/auth/logout` | User logout |
| `GET` | `/api/v1/auth/refresh` | Refresh access token |

### Student Management

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/students` | List all students |
| `POST` | `/api/v1/students` | Create new student |
| `PUT` | `/api/v1/students/:id` | Update student |
| `DELETE` | `/api/v1/students/:id` | Delete student |

### Notice Management

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/notices` | List notices |
| `POST` | `/api/v1/notices` | Create notice |
| `PUT` | `/api/v1/notices/:id` | Update notice |
| `DELETE` | `/api/v1/notices/:id` | Delete notice |

---

## 🤝 Contributing

1. Create a new repository on GitHub or Bitbucket.
2. Add your test result: `git add .`
3. Commit your changes: `git commit -m 'Add CRUD operations for test'`
4. Push to the branch: `git push origin main`

---

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](./LICENSE) file for details.

---

## 🆘 Support

For questions and support:

- Feel free to ask any questions anytime.
- Check existing documentation in [`/frontend/README.md`](./frontend/README.md) and [`/backend/README.md`](./backend/README.md).
- Review the database schema in [`/seed_db/tables.sql`](./seed_db/tables.sql).
