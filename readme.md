#  NuGet Packages

This project uses the following NuGet dependencies:

##  Core Frameworks
- **EntityFramework** — `6.5.1`  
  > Entity Framework 6 (EF6) is a tried and tested object-relational mapper for .NET.

- **Microsoft.EntityFrameworkCore** — `8.0.19`  
  > Modern object-database mapper for .NET with LINQ support, schema migrations, and multi-database compatibility.

- **Microsoft.EntityFrameworkCore.SqlServer** — `8.0.19`  
  > SQL Server database provider for Entity Framework Core.

- **Microsoft.EntityFrameworkCore.Tools** — `8.0.19`  
  > Tools for managing EF Core migrations and database updates via the NuGet Package Manager Console.

---

##  Authentication & Identity
- **Microsoft.AspNetCore.Authentication.JwtBearer** — `8.0.19`  
  > Middleware enabling applications to receive OpenID Connect bearer tokens.

- **Microsoft.AspNetCore.Identity.EntityFrameworkCore** — `8.0.19`  
  > ASP.NET Core Identity provider integrated with Entity Framework Core.

- **Microsoft.IdentityModel.Tokens** — `8.14.0`  
  > Provides support for security tokens and cryptographic operations (signing, verifying, encryption).

- **System.IdentityModel.Tokens.Jwt** — `8.14.0`  
  > Library for creating, serializing, and validating JSON Web Tokens (JWTs).  
  >  Legacy package — should be replaced with `Microsoft.IdentityModel.JsonWebTokens`.

---

##  Developer Tools
- **Microsoft.VisualStudio.Web.CodeGeneration.Design** — `8.0.7`  
  > Code generation tool for scaffolding ASP.NET Core controllers and views.

- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets** — `1.19.6`  
  > Enables Visual Studio container development and deployment.

---

##  Database & Drivers
- **Microsoft.SqlServer.Server** — `1.0.0`  
  > Helper library for `Microsoft.Data.SqlClient`, enabling cross-framework support of UDT types.

- **MongoDB.Driver** — `3.4.3`  
  > Official .NET driver for MongoDB.

- **MongoDB.Driver.Core** — `2.30.0`  
  > Core components of the official MongoDB .NET driver.

---

##  API & Documentation
- **Ocelot** — `24.0.1`  
  > API Gateway built on the .NET stack.

- **Swashbuckle.AspNetCore** — `6.4.0`  
  > Swagger tools for documenting ASP.NET Core APIs.

---

##  Others / Recommendations
- **Docker Desktop** — helps you run and visualize the containers.  
- **Postman** — useful for testing API endpoints.

---

##  Start the Application

1. Make sure **Docker Desktop** is running.  
2. Open a terminal at the root of the project.  
3. Run the following command:

   ```bash
   docker-compose up --build
   ```

This will build and start all the services defined in the `docker-compose.yml` file.

---

##  Stop the Application

To stop all running services, run:

```bash
docker-compose down
```

---

##  Microservices

The application consists of the following microservices:

- API Gateway  
- Identity  
- FrontEnd  
- Notes  
- Patients  

---

##  Entities

Main entities in this project are:

- User  
- Patient  
- Note  

---

##  Login

- Authentication is handled via **JWT tokens**.  
- Each service (except API Gateway and FrontEnd) requires a valid JWT token for access.  

**Default credentials for testing:**
- Username: `admin`  
- Password: `password`

**Authentication route:**
```
POST http://localhost:5000/identity/login
```

Use **Postman** or any API client to send a POST request with the credentials above to obtain a JWT token for authenticated requests.

---

##  Ports

The application uses the following ports:

| Service             | Port  |
|----------------------|-------|
| API Gateway          | `5000` |
| Identity Service     | `5001` |
| FrontEnd Service     | `5002` |
| Notes Service        | `5003` |
| Patients Service     | `5004` |
| MongoDB              | `27017` |
| SQL Server           | `1433` |

You can access services individually at:  
```
http://localhost:<port_number>/api/<service_name>/
```

Or through the API Gateway at:  
```
http://localhost:5000/<service_name>/
```

**Examples:**
- `http://localhost:5001/api/patients`  
- `http://localhost:5000/patients`
