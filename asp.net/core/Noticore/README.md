# NotiCore
An asynchronous notification engine built with .NET 10. It manages the full lifecycle of a notification, from persistence to multi-channel dispatching using Domain Events.

## Architecture & Project Structure

The project is built as an **ASP.NET Core API** following **Clean Architecture** using **CQRS** (MediatR) and **Domain Events** to decouple the notification lifecycle:

* **Noticore.Domain:** Core entities, Enums, and business rules.
* **Noticore.Application:** Use cases (Commands/Queries) and Async Event Consumers.
* **Noticore.Infrastructure:** Persistence via **EF Core** and external providers (Email/Fake).

## Local Development (No SMTP required)

I have implemented a **pre-configured development environment** so you don't need to set up a real SMTP server to test the end-to-end flow:

1. **Run the project:** `dotnet run --project Noticore.Api`
2. **Open Swagger:** `https://localhost:[PORT]/swagger`
3. **Test the flow:** Use the `POST /api/Notifications` endpoint with this sample JSON:

```json
{
  "title": "Welcome to NotiCore",
  "message": "Testing the infrastructure decoupling flow.",
  "recipient": "dev@example.com",
  "type": 0
}
```

## What happens behind the scenes?

* **Development Mode:** The system automatically injects a `FakeEmailService`. Instead of hitting a real SMTP gateway, a local folder named `/SentEmails` is created in the project root with a `.html` preview of the message.
* **Production Mode:** When running in a production environment, the system swaps the provider for a robust `SmtpEmailService`. This ensures real notifications are dispatched via an SMTP gateway, completing the professional lifecycle.
* **Async Lifecycle:** When a notification is requested, it is stored as `Pending`. A domain event is then published via MediatR and handled asynchronously, updating the status to `Sent` once the provider successfully processes the message.