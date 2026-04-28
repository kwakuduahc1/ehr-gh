# SHIMS - Simple Health Information System

A modern, cloud-ready Electronic Health Record (EHR) system built with .NET 10 and ASP.NET Core for managing patient medical records, insurance schemes, and healthcare operations.

## 🏥 Overview

SHIMS is a comprehensive health information management system designed to streamline patient registration, medical records management, attendance tracking, and insurance scheme handling. Built with modern cloud-native architecture and best practices, it's optimized for healthcare facilities of any size.

## ✨ Features

### Patient Management
- **Patient Registration** - Register new patients with personal and demographic information
- **Patient Profiles** - Manage detailed patient information including attendance history and insurance schemes
- **Search & Filter** - Search patients by name, hospital ID, or insurance card ID
- **Soft Delete** - Safe deletion marking records as inactive without permanent removal

### Insurance & Schemes
- **Scheme Management** - Define and manage insurance schemes
- **Patient Schemes** - Associate patients with multiple insurance schemes
- **Coverage Tracking** - Track insurance coverage and expiry dates

### API Features
- **RESTful API** - Clean, standard REST endpoints following HTTP conventions
- **API Versioning** - Built-in version management (v1.0+) for backward compatibility
- **OpenAPI/Swagger** - Interactive API documentation and testing
- **Response Caching** - 30-second cache on patient list endpoints for performance
- **JWT Authentication** - Secure bearer token-based authentication
- **Role-Based Authorization** - Fine-grained access control with multiple roles

### Technical Excellence
- **Async/Await** - Fully asynchronous operations for high performance
- **Cancellation Tokens** - Proper request cancellation support throughout
- **Connection Pooling** - Optimized database connection management
- **Comprehensive Logging** - Serilog integration with rolling file logs
- **Health Checks** - Built-in PostgreSQL and Redis health monitoring

## 🛠️ Tech Stack

| Component | Technology |
|-----------|-----------|
| **Framework** | ASP.NET Core 10 (.NET 10) |
| **Language** | C# 14.0 |
| **Database** | PostgreSQL 14+ |
| **ORM** | Dapper 2.1.72 |
| **Caching** | Valkey/Redis |
| **API Documentation** | Swagger/OpenAPI |
| **Authentication** | JWT Bearer Tokens |
| **API Versioning** | Asp.Versioning 10.0.0 |
| **Logging** | Serilog 4.3.1 |

## 🚀 Quick Start

### Prerequisites
- .NET 10 SDK or later
- PostgreSQL 14+
- Valkey/Redis (optional)

### Installation

1. **Clone repository**
```bash
git clone https://github.com/kwakuduahc1/ehr-gh.git
cd SHIMS
```

2. **Install dependencies**
```bash
dotnet restore
```

3. **Configure settings**
Create `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=shims;Username=postgres;Password=yourpassword",
    "Valkey": "localhost:6379"
  },
  "AppFeatures": {
    "Key": "your_jwt_secret_key_min_32_chars",
    "Issuer": "https://yourdomain.com",
    "Audience": "http://localhost:3000"
  }
}
```

4. **Run application**
```bash
dotnet run --project ShimsServer
```

Access Swagger UI: `https://localhost:7199/swagger`

## 📚 API Endpoints

### Patient Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/registrations` | Register new patient |
| `GET` | `/api/registrations` | Get all patients (cached 30s) |
| `GET` | `/api/registrations/{id}` | Get patient by ID |
| `GET` | `/api/registrations/details/{id}` | Get patient details with schemes |
| `GET` | `/api/registrations/search?search=term` | Search patients |
| `PUT` | `/api/registrations/{id}` | Update patient |
| `DELETE` | `/api/registrations/{id}` | Soft delete patient |

**Note**: All endpoints support versioning: `/api/v1.0/registrations`

## 🔐 Security

### Authentication
Include JWT token in Authorization header:
```
Authorization: Bearer <your_jwt_token>
```

### Authorization Roles
- **Doctor** - Medical staff
- **Nurse** - Nursing operations
- **Pharmacist** - Pharmacy operations
- **Billing** - Financial operations
- **Administration** - Administrative tasks
- **SysAdmin** - System administration

## 📊 Architecture

### Repository Pattern
Data access abstraction with async support and cancellation tokens.

### Dependency Injection
All services configured in `Program.cs` with proper lifetime management.

### Connection Pooling
- 16 concurrent database contexts (production-optimized)
- Automatic retry on transient failures
- Query splitting for complex operations

## 📝 Logging

Logs stored in `logs/` with daily rotation:
```bash
# View logs
tail -f logs/shims-*.txt
```

**Log Levels**:
- Development: Information+
- Production: Warning+
- Retention: 7 days

## 🧪 Development

### Code Standards
- C# 14.0 language features
- Positional records for DTOs
- Async/await for all I/O
- XML documentation on public APIs
- Validation attributes for model binding

### Building
```bash
dotnet build
```

### Testing
```bash
dotnet test
```

## 📦 Deployment

Ready for:
- **Docker** - Container deployment (coming soon)
- **Azure App Service** - Cloud hosting
- **Azure SQL Database** - Managed database
- **Azure Cache for Redis** - Managed caching

## 🔄 API Versioning

Both formats are supported:
- Versioned: `GET /api/v1.0/registrations`
- Unversioned (defaults to v1.0): `GET /api/registrations`

## 📧 Support & Issues

- **GitHub Issues**: Report bugs and request features
- **Email**: support@bstudio.com
- **Documentation**: https://docs.bstudio.com

## 📄 License

MIT License - See LICENSE file for details

## 🎯 Roadmap

- [ ] Advanced analytics and reporting
- [ ] Mobile app integration  
- [ ] Telemedicine features
- [ ] FHIR compliance
- [ ] Audit logging enhancements
- [ ] Multi-language support

---

**SHIMS** - Simplifying Healthcare Information Management