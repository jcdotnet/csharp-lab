# Cities Manager API

An implementation of a RESTful API using **ASP.NET Core Web API**. This project acts as the backend for the [Angular Cities Client](https://github.com/jcdotnet/angular-lab/tree/main/cities-client-app).

### Implementation details
* **Identity & Security:** ASP.NET Core Identity integration with JWT.
* **Storage:** EF Core with SQL Server using GUIDs.
* **Architecture:** Unlike other projects over here, this one avoids the Repository/Service pattern to **reduce over-engineering**, focusing instead on security patterns and identity management.
* **CORS:** Configured for decoupled frontend integration.

## Local Development Setup
Run the Microsoft SQL Server database locally with Docker.

```bash
# Download and start MS SQL Server in a container
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<password>" -p 1433:1433 --name cities-db -d mcr.microsoft.com/mssql/server:2025-latest
# Check if the container is running
docker ps
