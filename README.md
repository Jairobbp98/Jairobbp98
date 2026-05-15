# 🏢 Full-Stack Management System

A modern, full-stack management system built with **.NET MAUI**, **NestJS**, and **PostgreSQL**. Features user and company management, document generation (PDF & Markdown), JWT authentication, role-based access control, and Docker-based deployment — all inspired by Notion's clean UI design.

---

## 🏗️ Architecture

```
├── backend/                   # NestJS REST API
│   ├── src/
│   │   ├── auth/              # JWT authentication & guards
│   │   ├── users/             # User management module
│   │   ├── companies/         # Company management module
│   │   ├── documents/         # Document generation (PDF & MD)
│   │   └── common/            # Decorators, guards, utilities
│   ├── Dockerfile
│   └── package.json
│
├── mobile/                    # .NET MAUI cross-platform app
│   ├── src/
│   │   ├── Pages/             # XAML UI pages
│   │   ├── ViewModels/        # MVVM ViewModels (CommunityToolkit)
│   │   ├── Services/          # API & Auth services
│   │   ├── Models/            # Domain models
│   │   └── Converters/        # Value converters
│   └── MauiProgram.cs
│
├── docker-compose.yml         # Full stack deployment
└── .github/workflows/         # GitHub Actions CI/CD
```

---

## 🚀 Tech Stack

| Layer | Technology |
|-------|------------|
| **Mobile** | .NET MAUI 8, MVVM, CommunityToolkit.Maui |
| **Backend** | NestJS 10, TypeScript, Passport.js |
| **Database** | PostgreSQL 16, TypeORM |
| **Auth** | JWT (Access + Refresh tokens) |
| **Docs** | PDFKit (PDF), Marked (Markdown) |
| **API Docs** | Swagger / OpenAPI |
| **Containers** | Docker, Docker Compose |
| **CI/CD** | GitHub Actions |

---

## ✨ Features

### 🔐 Authentication & Authorization
- JWT-based authentication with access (15min) and refresh (7d) tokens
- Role-based access control: **Admin**, **Manager**, **Employee**, **Viewer**
- Secure password hashing with bcrypt (12 rounds)
- Session persistence in .NET MAUI with SecureStorage

### 👥 User Management
- Full CRUD operations for users
- Role assignment and management
- Account activation/deactivation
- Paginated user listing with filters

### 🏢 Company Management
- Create and manage company profiles
- Industry classification and contact details
- Associate users with companies
- Paginated company listing

### 📄 Document Generation
- Create documents in **Markdown** or **PDF** format
- Export any document as a styled **PDF** (via PDFKit)
- Export any document as **Markdown** with YAML frontmatter
- Document lifecycle: Draft → Published → Archived
- Tag-based organization

### 📱 Mobile App (MAUI)
- **Notion-inspired** clean UI with cards and minimal design
- Dashboard with real-time stats (users, companies, documents)
- Swipe actions for quick PDF/Markdown export
- Pull-to-refresh throughout
- Deep linking with Shell navigation

---

## 🐳 Quick Start with Docker

### Prerequisites
- Docker Engine 24+
- Docker Compose v2

### Run the full stack

```bash
# Clone the repository
git clone https://github.com/Jairobbp98/Jairobbp98.git
cd Jairobbp98

# Copy environment file
cp backend/.env.example backend/.env
# Edit backend/.env and set your secrets (especially JWT_SECRET)

# Start all services
docker compose up -d

# View logs
docker compose logs -f backend
```

The API will be available at:
- **API:** `http://localhost:3000/api/v1`
- **Swagger:** `http://localhost:3000/api/docs`

---

## 🛠️ Local Development

### Backend (NestJS)

```bash
cd backend

# Install dependencies
npm install

# Copy environment file
cp .env.example .env

# Start PostgreSQL (via Docker)
docker run -d \
  --name management_db \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=management_db \
  -p 5432:5432 \
  postgres:16-alpine

# Start in development mode
npm run start:dev

# Run tests
npm test

# Run tests with coverage
npm run test:cov
```

### Mobile (.NET MAUI)

```bash
cd mobile

# Restore packages
dotnet restore

# Run on Android
dotnet build -t:Run -f net8.0-android

# Run on iOS
dotnet build -t:Run -f net8.0-ios

# Run on Windows
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```

> Update `MauiProgram.cs` to point `apiBaseUrl` to your backend server's IP/hostname.

---

## 📡 API Reference

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/v1/auth/register` | Register a new user |
| `POST` | `/api/v1/auth/login` | Login and get tokens |
| `POST` | `/api/v1/auth/refresh` | Refresh access token |
| `GET`  | `/api/v1/auth/profile` | Get current user |

### Users
| Method | Endpoint | Access |
|--------|----------|--------|
| `GET`  | `/api/v1/users` | All roles |
| `POST` | `/api/v1/users` | Admin, Manager |
| `GET`  | `/api/v1/users/:id` | All roles |
| `PATCH`| `/api/v1/users/:id` | All roles |
| `DELETE`| `/api/v1/users/:id` | Admin only |

### Companies
| Method | Endpoint | Access |
|--------|----------|--------|
| `GET`  | `/api/v1/companies` | All roles |
| `POST` | `/api/v1/companies` | Admin, Manager |
| `PATCH`| `/api/v1/companies/:id` | Admin, Manager |
| `DELETE`| `/api/v1/companies/:id` | Admin only |

### Documents
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET`  | `/api/v1/documents` | List documents |
| `POST` | `/api/v1/documents` | Create document |
| `PATCH`| `/api/v1/documents/:id` | Update document |
| `GET`  | `/api/v1/documents/:id/export/pdf` | Export as PDF |
| `GET`  | `/api/v1/documents/:id/export/markdown` | Export as Markdown |
| `PATCH`| `/api/v1/documents/:id/publish` | Publish document |
| `PATCH`| `/api/v1/documents/:id/archive` | Archive document |

---

## 🧪 Testing

```bash
# Unit tests
cd backend && npm test

# Coverage report
npm run test:cov
```

---

## 🔄 CI/CD Pipeline

### CI Workflow (`.github/workflows/ci.yml`)
Triggered on every push/PR to `main` and `develop`:
1. **Lint** — ESLint with TypeScript
2. **Unit Tests** — Jest with PostgreSQL service
3. **Build** — TypeScript compilation
4. **Docker Build** — Validates Dockerfile

### CD Workflow (`.github/workflows/cd.yml`)
Triggered on push to `main` or version tags:
1. **Build Docker image** — Multi-arch (amd64 + arm64)
2. **Push to GitHub Container Registry** (`ghcr.io`)
3. **Deploy to Staging** — On `main` branch pushes
4. **Deploy to Production** — On version tag pushes (e.g., `v1.0.0`)

---

## 🔒 Security

- All passwords hashed with bcrypt (12 rounds)
- JWT access tokens expire in 15 minutes
- JWT refresh tokens expire in 7 days
- Role-based route guards on all protected endpoints
- Input validation with `class-validator`
- CORS configured per environment
- Database synchronization disabled in production

---

## 📁 Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `NODE_ENV` | Environment | `development` |
| `PORT` | Server port | `3000` |
| `DB_HOST` | PostgreSQL host | `localhost` |
| `DB_PORT` | PostgreSQL port | `5432` |
| `DB_USERNAME` | DB username | `postgres` |
| `DB_PASSWORD` | DB password | `postgres` |
| `DB_NAME` | Database name | `management_db` |
| `JWT_SECRET` | JWT signing secret | *(required)* |
| `CORS_ORIGIN` | Allowed CORS origin | `*` |

---

## 📄 License

MIT © [Jairobbp98](https://github.com/Jairobbp98)
