# Cities Manager Web API

An implementation of a RESTful API using **ASP.NET Core Web API**. This project acts as the backend for the [Angular Cities Client](https://github.com/jcdotnet/angular-lab/tree/main/cities-client-app).

### Implementation details
* **Identity & Security:** ASP.NET Core Identity integration with JWT.
* **Storage:** EF Core with SQL Server using GUIDs.
* **Architecture:** Unlike other projects over here, this one avoids the Repository/Service pattern to **reduce over-engineering**, focusing instead on security patterns and identity management.
* **CORS:** Configured for decoupled frontend integration.


## Local Development Setup
Run the Microsoft SQL Server database and the Cities Manager Web API with Docker.

Create the images and start the containers
```bash
docker compose up -d --build
```
Apply database migrations via .NET cli tools
```bash
dotnet ef database update --project CitiesManager.Infrastructure --startup-project CitiesManager.WebAPI
```
