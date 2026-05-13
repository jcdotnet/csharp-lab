# Noticore

Async notifications for .NET 10

## Architecture & Project Structure

I created this project to experiment with decoupling using MediatR and Domain Events instead of a traditional service layer.

It is built as an **ASP.NET Core Web API** following **Clean Architecture** using **CQRS** (MediatR) and **Domain Events** to decouple the notification lifecycle:

* **Noticore.Domain:** Core entities, Enums, and business rules.
* **Noticore.Application:** Use cases (Commands/Queries) and Async Event Consumers.
* **Noticore.Infrastructure:** Persistence via **EF Core** and external providers (Email/Fake).

When running in a production environment, the system swaps the provider for a robust `SmtpEmailService`. This ensures real notifications are dispatched via an SMTP gateway, completing the professional lifecycle.

* **Async Lifecycle:** When a notification is requested, it is stored as `Pending`. A domain event is then published via MediatR and handled asynchronously, updating the status to `Sent` once the provider successfully processes the message.

**Quick Notes:**
* *MediatR:* I am aware that using MediatR for a simple notification flow might be over-engineering. In this case, a traditional service layer might be more straightforward.
* *In-Memory:* Since everything runs in the application's memory, if it crashes or restarts before sending the email, pending emails are lost. If I need a safe solution later, I will study background tasks or message queues.

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

Development Mode: The system automatically injects a `FakeEmailService`. Instead of hitting a real SMTP gateway, a local folder named `/SentEmails` is created in the project root with a `.html` preview of the message.
