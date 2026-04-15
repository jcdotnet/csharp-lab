# Cities Manager API

An implementation of a RESTful API using **ASP.NET Core Web API**. This project acts as the backend for the [Angular Cities Client](https://github.com/jcdotnet/angular-lab/tree/main/cities-client-app).

### Implementation details
* **Identity & Security:** ASP.NET Core Identity integration with JWT.
* **Storage:** EF Core with SQL Server using GUIDs.
* **Architecture:** Unlike other projects over here, this one avoids the Repository/Service pattern to **reduce over-engineering**, focusing instead on security patterns and identity management.
* **CORS:** Configured for decoupled frontend integration.