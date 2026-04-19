# Cloud-Native eCommerce App

Distributed microservices lab using ASP.NET Core. Each service follows a different architectural pattern and database engine depending on requirements. I use this project to test different architectures and automate the deployment with Azure DevOps pipelines.

### Services
* **Users Service**: Clean Architecture | Postgres + Dapper.
  * *CI/CD & Infrastructure:* [Azure DevOps Repo](https://github.com/jcdotnet/azure-devops-users-microservice)
* **Products Service**: N-Tier | MySQL + EF Core.
  * *CI/CD & Infrastructure:* [Azure DevOps Repo](https://github.com/jcdotnet/azure-devops-products-microservice)
* **Orders Service**: N-Tier | MongoDB + EF Core.
  * *CI/CD & Infrastructure:* [Azure DevOps Repo](https://github.com/jcdotnet/azure-devops-orders-microservice)

### Project Evolution
I used different branches to show how the app grows from basic containers to a full cloud setup.
1.  **Communication**: Migration from Synchronous (HTTP) to Asynchronous messaging and API Gateway integration
2.  **Deployment**: Evolution from local **Docker** to **Kubernetes** and finally **Azure AKS**.