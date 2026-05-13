# Cloud-Native eCommerce App

This project was built as part of the Udemy course **[.NET Microservices with Azure DevOps & AKS](https://www.udemy.com/course/dot-net-microservices-ecommerce-project-azure-devops-kubernetes-aks/)** to experiment with microservices architectures and Azure DevOps automation. I used to see how different patterns and database engines work together within a distributed system.

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