# Railway Management System with Agentic AI

## 📋 Project Overview

A comprehensive full-stack Railway Management System powered by Agentic AI, enabling users to book and manage train tickets through an intelligent chatbot interface. The system integrates a Python-based AI agent backend with a .NET Core REST API and an Angular frontend, creating a seamless omnichannel experience for railway operations.

**Key Highlight:** This system demonstrates advanced AI agent orchestration using LangGraph, with multi-assistant routing for specialized domain tasks (reservations, train management, and user services).

---

## 🏗️ Architecture & Components

### 1. **ChatBot (Python - Agentic AI Backend)**
**Location:** `ChatBot/`

An intelligent conversational AI agent built with FastAPI and LangGraph that handles multi-turn conversations and routes requests to specialized assistants.

**Core Technologies:**
- **FastAPI**: High-performance async web framework
- **LangChain & LangGraph**: AI orchestration and agentic workflow management.  
- **Azure OpenAI / Google Gemini**: Large Language Models (configurable)
- **MongoDB Atlas**: Document storage and vector database with Atlas Search
- **Motor**: Async MongoDB driver

**Key Features:**
- Multi-assistant architecture with intelligent routing
- ReservationAssistant: Manages ticket bookings and cancellations
- TrainAssistant: Handles train information and management
- UserAssistant: Manages user registration and authentication
- Stateful conversations with MongoDB checkpoints
- Support for both Azure OpenAI and Google Gemini endpoints

**APIs:**
- `POST /graph/async-agent`: Process user questions through the agentic workflow
- `POST /auth/validate-token`: JWT token validation
- `POST /doc-loader/upload`: Upload and process documents
- `GET/POST /checkpoint`: Checkpoint management for conversation state

**Models & Tools:**
- `BookTicket`: Ticket reservation with passenger information
- `CancellationDTO`: Ticket cancellation requests
- `PassengerDTO`: Passenger details (name, age, gender, Aadhar ID)
- Specialized tools for train operations, reservations, and user management

---

### 2. **RailwayBackend (.NET Core REST API)**
**Location:** `RailwayBackend/`

A robust, enterprise-grade backend built with ASP.NET Core 8, following clean architecture principles with business, repository, and model layers.

**Architecture Layers:**
- **PresentationLayer** (`RailwayManagmentSystem`): Controllers and API endpoints
- **BusinessLayer** (`BusinessLayer`): Business logic, JWT token generation, email service
- **RepositoryLayer** (`RepositoryLayer`): Data access, Entity Framework migrations
- **ModelLayer** (`ModelLayer`): DTOs and data contracts

**Core Technologies:**
- **ASP.NET Core 8.0**: Latest .NET framework
- **Entity Framework Core**: ORM for database operations
- **SQL Server**: Primary database
- **JWT Authentication**: Secure token-based authentication
- **AutoMapper**: Object-relational mapping
- **Swagger/OpenAPI**: API documentation

**Database Entities:**
- `User`: User accounts and profiles
- `Train`: Train information (routes, schedules)
- `TrainClass`: Compartment types (AC, Non-AC, Sleeper)
- `Reservation`: Ticket bookings
- `Passenger`: Passenger details
- `Cancellation`: Cancellation records

**Controllers:**
- `RailwayTicketBookingController`: Reservation and ticket operations
- `TrainController`: Train management and search

**Services:**
- `RailwayBusinessLayer`: Core business logic
- `TrainBL`: Train-specific operations
- `EmailService`: SMTP-based notifications
- `JWTToken`: Token generation and validation

**Configuration:**
```json
{
  "ConnectionStrings": {
    "SqlConnection": "SQL Server connection string"
  },
  "JWT": {
    "Key": "Secret key for token signing",
    "Issuer": "Token issuer",
    "Audience": "Token audience",
    "ExpireMinutes": 60
  },
  "SMTP": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "Email service account",
    "Password": "App password"
  }
}
```

---

### 3. **RailwayFrontend (Angular 16)**
**Location:** `RailwayFrontend/`

A responsive, feature-rich single-page application for user-friendly railway management operations.

**Core Technologies:**
- **Angular 16**: Modern TypeScript-based framework
- **Bootstrap 5**: Responsive UI components
- **RxJS**: Reactive programming for state management
- **JWT-decode**: Client-side token parsing

**Key Features:**
- User authentication and role-based access control
- Train search and discovery
- Ticket booking and payment integration
- Reservation management
- Ticket cancellation
- Admin dashboard for train and reservation management
- Integrated chatbot interface
- PDF ticket generation (jsPDF)

**Main Components:**
- `login/`: User authentication
- `register/`: New user registration
- `homepage/`: Landing page
- `searchtrain/`: Train search interface
- `addreservation/`: Booking workflow
- `allreservations/`: User's booking history
- `cancellation-ticket/`: Cancellation interface
- `chatbot/`: Integrated AI chat interface
- `admindashboard/`: Admin control panel
  - `addtrain/`, `updatetrain/`, `canceltrain/`
  - `admindetails/` with sidebar navigation
- `userdashboard/`, `userdetails/`: User profile management
- `trainchart/`: Visual train information

**Dependencies:**
- Angular CLI 16.2.16
- Bootstrap 5.3.6
- jsPDF 3.0.1
- jwt-decode 4.0.0

---

### 4. **Docker Deployment**
**File:** `Dockerfile`

Multi-stage build configuration for containerized deployment of the .NET backend.

**Build Stages:**
1. **Build Stage**: .NET 8.0 SDK - compiles C# source
2. **Publish Stage**: Publishes optimized release builds
3. **Runtime Stage**: .NET 8.0 ASP.NET runtime - minimal production image

**Exposed Ports:** 80 (HTTP), 443 (HTTPS)

**Entry Point:** `RailwayManagmentSystem.dll`

---

## 🚀 Key Features

### For Users:
✅ **Intelligent Chatbot**: Natural language interaction for all railway operations  
✅ **Ticket Booking**: Multi-passenger bookings with various train classes  
✅ **Ticket Cancellation**: Easy refund processing  
✅ **Train Search**: Filter by route, date, class, and availability  
✅ **Reservation History**: View all past and upcoming bookings  
✅ **Email Notifications**: Booking confirmations and cancellation receipts  
✅ **JWT-based Authentication**: Secure account access  

### For Admins:
✅ **Train Management**: Add, update, and manage trains  
✅ **Dashboard Analytics**: Reservation statistics and insights  
✅ **User Management**: Monitor user accounts and activity  
✅ **Role-based Access Control**: Different permissions for users and admins  

### Technical Highlights:
✅ **Agentic AI Workflow**: Multi-assistant LangGraph implementation  
✅ **Stateful Conversations**: MongoDB-backed conversation history  
✅ **Dual LLM Support**: Azure OpenAI or Google Gemini endpoints  
✅ **Vector Search**: MongoDB Atlas Vector Search for intelligent document retrieval  
✅ **Scalable Architecture**: Microservices-ready design  
✅ **Comprehensive Testing**: Unit tests for business logic and controllers  

---

## 📦 Dependencies Summary

### ChatBot (Python)
```
FastAPI >= 0.104.1
uvicorn >= 0.24.0
Motor >= 3.3.2 (Async MongoDB)
PyMongo >= 4.6.0
LangChain >= 0.1.0
LangGraph >= 0.0.55
LangChain-OpenAI (Azure OpenAI integration)
LangChain-Google-GenAI (Google Gemini integration)
LangChain-MongoDB >= 0.1.3
Pydantic >= 2.5.0
Python-Jose (JWT handling)
Passlib + Bcrypt (Password hashing)
httpx >= 0.24.0 (Async HTTP client)
Pillow >= 10.0.0 (Image processing)
BeautifulSoup4 >= 4.12.0 (Document parsing)
```

### RailwayBackend (.NET)
```
.NET 8.0 Runtime / SDK
Entity Framework Core (Latest)
JWT Bearer Authentication
AutoMapper
Swagger/Swashbuckle
System.IdentityModel.Tokens.Jwt
Microsoft.SqlServer.Management.SqlParser
```

### RailwayFrontend (Angular)
```
Angular 16.2.0
TypeScript 5.1.3
Bootstrap 5.3.6
RxJS 7.8.0
jsPDF 3.0.1
jwt-decode 4.0.0
```

---

## 🔧 Setup Requirements

### Prerequisites:
- **Python 3.9+** (for ChatBot)
- **.NET 8.0 SDK** (for RailwayBackend)
- **Node.js 18+ & npm** (for RailwayFrontend)
- **SQL Server** (for database)
- **MongoDB Atlas** (for vector storage and checkpoints)
- **Docker** (optional, for containerization)
- **Azure OpenAI API Key** OR **Google Gemini API Key**

### Environment Variables (ChatBot/.env):
```
mongo_uri=<MongoDB connection string>
DB_NAME=capstone_project
DOCUMENTS_COLLECTION=documents
ATLAS_VECTOR_SEARCH_INDEX_NAME=vector_capstone

# Azure OpenAI Configuration (if using Azure)
AZURE_API_KEY=<your-key>
AZURE_ENDPOINT=<your-endpoint>
AZURE_API_VERSION=<version>
AZURE_DEPLOYMENT=<deployment-name>
AZURE_EMBEDDING_DEPLOYMENT=<embedding-deployment>
AZURE_EMBEDDING_MODEL=<model-name>

# Google Gemini Configuration (if using Gemini)
GOOGLE_API_KEY=<your-key>
GOOGLE_LLM_MODEL=gemini-pro
GOOGLE_EMBEDDING_MODEL=models/embedding-001
```

### Configuration (RailwayBackend/appsettings.json):
```json
{
  "ConnectionStrings": {
    "SqlConnection": "<SQL Server connection string>"
  },
  "JWT": {
    "Key": "<256-bit secret key>",
    "Issuer": "Railway.API",
    "Audience": "Railway.Users",
    "ExpireMinutes": 60
  },
  "SMTP": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "<email@gmail.com>",
    "Password": "<app-password>"
  }
}
```

---

## 📡 API Integration Flow

```
Frontend (Angular) 
    ↓
Journey A: REST API Calls → RailwayBackend (.NET) → SQL Server
    ↓
Journey B: ChatBot Messages → ChatBot (Python) → LangGraph Agents
    ↓
    → Tools call RailwayBackend API (http://localhost:5063/api/Train)
    ↓
    → Response returned as natural language to user
```

---

## 🔐 Security Features

- **JWT Token-based Authentication**: Secure APIs with Bearer tokens
- **Role-based Access Control (RBAC)**: User vs Admin roles
- **Password Hashing**: Bcrypt with Passlib
- **CORS Configuration**: Whitelist approved origins
- **Email Verification**: SMTP-based confirmations
- **Encrypted Credentials**: Environment-based secret management

---

## 🧪 Testing

**Test Project:** `RailwayManagmentTesting/`

- Unit tests for BusinessLayer services
- Integration tests for API controllers
- JWT and Email service validation
- Model validation tests
- Test batch scripts: `RunAllTests.bat`, `RunUnitTests.bat`

---

## 📬 Contact & Support

**Developer:** Akash Singh  
**Email:** akash.si8273@gmail.com

-----
## 📝 License

[Specify your license here]

---

This comprehensive summary provides a complete overview of your Railway Management System suitable for documentation, onboarding, and deployment purposes.
