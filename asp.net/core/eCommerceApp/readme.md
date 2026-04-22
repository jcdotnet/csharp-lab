# Cloud-Native eCommerce App

This is my playground for testing microservices architectures and Azure DevOps automation. I use it to see how different patterns and database engines work together within a distributed system."

### Services
* **Users Service**: Clean Architecture | Postgres + Dapper.
  * *CI/CD & Infrastructure:* [Azure DevOps Repo](https://github.com/jcdotnet/azure-devops-users-microservice)
* **Products Service**: N-Tier | MySQL + EF Core.
  * *CI/CD & Infrastructure:* [Azure DevOps Repo](https://github.com/jcdotnet/azure-devops-products-microservice)
* **Orders Service**: N-Tier | MongoDB + EF Core.
  * *CI/CD & Infrastructure:* [Azure DevOps Repo](https://github.com/jcdotnet/azure-devops-orders-microservice)

### Project 
This project is structured to show the architectural transition from basic containers to a cloud setup. I've used different **feature branches** to document key milestones:

* **Integration**: Transition from synchronous communication to asynchronous messaging and API Gateway patterns.
* **Deployment**: Evolution from local Docker environments to Kubernetes and finally AKS.

Note: Some parts of this lab (like auth or security) are simplified for testing purposes and do not follow production standards.