# Noticore (notifications + .NET Core)

I created this project as a LAB to experiment with decoupling using MediatR and Domain Events instead of a service layer.

I'm aware that this approach could be considered overengineering for such a simple notification app, but that is completely intentional here. 

## Clean Architecture
* **Noticore.Domain:** Entities, enums and business rules.
* **Noticore.Application:** Commands, queries and event handlers.
* **Noticore.Infrastructure:** EF Core persistence and notification services (real and fake for dev)

## Notifications

Every notification is stored in the database with `Pending` status.

Then the notification is processed and the email is sent:

- If the email is sent successfully, the status becomes `Sent`.
- If the email fails after the retries, the status becomes `Failed`.

Note: SMS and push notifications not implemented yet.

## Email service

I have implemented a fake email service for development, so you don't need to set up a real SMTP server to test the application.

## Polly

The SMTP service uses Polly to experiment with retry and resilience.

The current retry configuration uses exponential backoff : 2s -> retry -> 4s -> retry -> 8s -> ...

To test the retry behavior, I can temporarily uncomment this line in `SmtpEmailService`:

`throw new Exception("Temporary connection issue");`

## Local Development (No SMTP required)

1. Run the API: `dotnet run --project Noticore.Api`
2. Open Swagger: `https://localhost:[PORT]/swagger`
3. Create a notification using POST /api/Notifications:

```json
{
  "title": "Test notification",
  "message": "Test message",
  "recipient": "dev@example.com",
  "type": 0
}
```

Note: In development, FakeEmailService writes the generated email to the SentEmails folder instead of sending it through SMTP.

Note: If the database is missing or the migrations have not been applied:

`dotnet ef database update --project Noticore.Infrastructure --startup-project Noticore.Api`