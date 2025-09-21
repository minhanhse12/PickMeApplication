# PickMe Application - Microservices Architecture

A food delivery application that allows users to pre-order and pick up food from restaurants, built using microservices architecture with .NET 9 and the Saga pattern.

## 🏗️ Architecture Overview

This application follows a **microservices architecture** with the **Database per Service** pattern, implementing **Domain-Driven Design (DDD)** and **Clean Architecture** principles.

### 🎯 Business Domain

- **3 User Roles**: Admin, Customer, Restaurant Owner
- **Admin**: Manages campaigns, promotions, and system information
- **Restaurant Owners**: Provide restaurant information, menu items, and receive orders
- **Customers**: Browse restaurants, view menus, get route suggestions, place orders, and schedule pickup times

### 🔧 Core Services

| Service | Port | Database | Description |
|---------|------|----------|-------------|
| **User Service** | 5024 | `PickMeUserService_Dev` | Authentication, user management, roles |
| **Restaurant Service** | TBD | `PickMeRestaurantService_Dev` | Restaurant profiles, locations |
| **Menu Service** | TBD | `PickMeMenuService_Dev` | Menu items, pricing, availability |
| **Order Service** | TBD | `PickMeOrderService_Dev` | Order processing, pickup scheduling |
| **Payment Service** | TBD | `PickMePaymentService_Dev` | Payment processing, transactions |
| **Location Service** | TBD | `PickMeLocationService_Dev` | Route suggestions, GPS integration |
| **Campaign Service** | TBD | `PickMeCampaignService_Dev` | Promotions, discounts |
| **Notification Service** | TBD | `PickMeNotificationService_Dev` | Email, SMS, push notifications |
| **AI Service** | TBD | `PickMeAIService_Dev` | Time optimization, route intelligence |

### 🛠️ Infrastructure Services

- **API Gateway** (Ocelot) - Single entry point, authentication, load balancing
- **Saga Orchestrator** - Manages distributed transactions
- **Event Bus** (RabbitMQ/Azure Service Bus) - Asynchronous communication

## 🏛️ Clean Architecture Structure

Each microservice follows Clean Architecture with these layers:

```
Service/
├── Service.API/           # Controllers, Endpoints, Configuration
├── Service.Application/   # Business Logic, DTOs, Interfaces
├── Service.Domain/        # Entities, Domain Models, Repository Interfaces
└── Service.Infrastructure/ # Data Access, External Services
```

## 🗄️ Database Strategy

- **Database per Service**: Each microservice has its own MySQL database
- **Event Sourcing**: For audit trails and order tracking
- **CQRS**: Read/Write separation for complex queries

## 🚀 Getting Started

### Prerequisites

- .NET 9 SDK
- MySQL Server 8.0+
- Docker (optional)
- Visual Studio 2022 or VS Code

### Running the Application

1. **Clone the repository**
   ```bash
   git clone https://github.com/minhanhse12/PickMeApplication.git
   cd PickMeApplication
   ```

2. **Setup MySQL Database**
   ```bash
   # Update connection string in appsettings.Development.json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Port=3306;Database=PickMeUserService_Dev;User=root;Password=yourpassword;"
   }
   ```

3. **Run Database Migrations**
   ```bash
   dotnet ef database update --project "src/Services/UserService/PickMeApplication.UserService.Infrastructure" --startup-project "src/Services/UserService/PickMeApplication.UserService.API"
   ```

4. **Start User Service**
   ```bash
   dotnet run --project "src/Services/UserService/PickMeApplication.UserService.API/PickMeApplication.UserService.API.csproj"
   ```

5. **Access Swagger UI**
   - User Service: http://localhost:5024/swagger

## 🧪 Testing

### API Testing

Use the provided `.http` files in each service's API project:
- `UserService.http` - Test user operations

### Available Endpoints (User Service)

- `POST /api/users` - Register new user
- `POST /api/users/login` - User authentication
- `GET /api/users` - Get all users (Admin only)
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `POST /api/users/{id}/change-password` - Change password

## 🔄 Saga Pattern Implementation

The application uses the **Saga pattern** for managing distributed transactions:

### Example: Order Placement Saga
1. **Order Created** → Order Service
2. **Payment Processing** → Payment Service  
3. **Restaurant Notification** → Notification Service
4. **Menu Item Reservation** → Menu Service
5. **Location Calculation** → Location Service
6. **Order Confirmation** → All Services

**Compensation**: If any step fails, previous steps are automatically rolled back.

## 🛡️ Security

- **JWT Authentication** with role-based authorization
- **BCrypt** password hashing
- **HTTPS** enforcement
- **Input validation** and sanitization

## 📊 Technology Stack

- **Backend**: ASP.NET Core 9.0, Entity Framework Core
- **Database**: MySQL 8.0
- **Authentication**: JWT Bearer tokens
- **API Documentation**: Swagger/OpenAPI
- **Message Broker**: RabbitMQ (planned)
- **API Gateway**: Ocelot (planned)
- **Containerization**: Docker (planned)

## 🎯 Key Features

- 🏪 **Route-based Restaurant Suggestions**: AI-powered recommendations based on user's route
- ⏰ **Smart Pickup Scheduling**: AI optimization for pickup times
- 📱 **Multi-platform Support**: Web API ready for mobile and web clients
- 🔄 **Event-driven Architecture**: Asynchronous communication between services
- 📊 **Comprehensive Monitoring**: Built-in logging and health checks

## 🚧 Current Status

- ✅ **User Service**: Complete (Authentication, User Management)
- 🚧 **Restaurant Service**: In development
- ⏳ **Other Services**: Planned
- ⏳ **API Gateway**: Planned
- ⏳ **Docker Support**: Planned

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Contact

- **Email**: support@pickmeapp.com
- **Project Repository**: [https://github.com/minhanhse12/PickMeApplication](https://github.com/minhanhse12/PickMeApplication)