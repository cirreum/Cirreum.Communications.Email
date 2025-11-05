# Cirreum.Communications.Email

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Communications.Email.svg?style=flat-square)](https://www.nuget.org/packages/Cirreum.Communications.Email/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Communications.Email.svg?style=flat-square)](https://www.nuget.org/packages/Cirreum.Communications.Email/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Communications.Email?style=flat-square)](https://github.com/cirreum/Cirreum.Communications.Email/releases)

Core abstractions and models for email communication services within the Cirreum ecosystem.

## Overview

This package provides the fundamental interfaces and data models for email messaging functionality. It defines contracts that email provider implementations must follow, enabling consistent email operations across different provider backends.

## Installation

```bash
dotnet add package Cirreum.Communications.Email
```

## Interfaces

### IEmailService

The primary interface for email operations supporting individual messages, template-based bulk sending, and personalized bulk messaging.

```csharp
public interface IEmailService
{
    /// <summary>
    /// Sends a single email.
    /// </summary>
    Task<EmailResult> SendEmailAsync(EmailMessage message, CancellationToken ct = default);

    /// <summary>
    /// Sends bulk emails to multiple recipients using a shared template.
    /// </summary>
    Task<EmailResponse> SendBulkEmailAsync(
        EmailMessage template,
        IEnumerable<EmailAddress> recipients,
        bool validateOnly = false,
        CancellationToken ct = default);

    /// <summary>
    /// Sends bulk emails with individual personalization.
    /// </summary>
    Task<EmailResponse> SendBulkEmailAsync(
        IEnumerable<EmailMessage> messages,
        bool validateOnly = false,
        CancellationToken ct = default);
}
```

## Data Models

### EmailAddress

Represents an email address with an optional display name.

```csharp
public readonly record struct EmailAddress(string Address, string? Name = null);
```

### EmailResult

Represents the result of a single email operation.

```csharp
public sealed record EmailResult
{
    public required string EmailAddress { get; init; }
    public required bool Success { get; init; }
    public string? MessageId { get; init; }
    public string? ErrorMessage { get; init; }
    public string? Provider { get; init; }
    public int? StatusCode { get; init; }
    public TimeSpan? RetryAfter { get; init; }
    public string? CirreumelationId { get; init; }
    public IList<string> ValidationErrors { get; init; } = new List<string>();
}
```

### EmailResponse

Represents the aggregate result of bulk email operations.

```csharp
public sealed record EmailResponse(int Sent, int Failed, IReadOnlyList<EmailResult> Results);
```

### EmailMessage

Comprehensive email message model supporting rich content and metadata.

```csharp
public sealed record EmailMessage
{
    public required EmailAddress From { get; init; }
    public EmailAddress? ReplyTo { get; init; }
    public IList<EmailAddress> To { get; init; } = new List<EmailAddress>();
    public IList<EmailAddress> Cc { get; init; } = new List<EmailAddress>();
    public IList<EmailAddress> Bcc { get; init; } = new List<EmailAddress>();
    public string? Subject { get; init; }
    public string? TextContent { get; init; }
    public string? HtmlContent { get; init; }
    public IList<EmailAttachment> Attachments { get; init; } = new List<EmailAttachment>();
    public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
    public IList<string> Categories { get; init; } = new List<string>();
    public IDictionary<string, string> CustomArgs { get; init; } = new Dictionary<string, string>();
    public EmailPriority Priority { get; init; } = EmailPriority.Normal;
    public string? TemplateKey { get; init; }
    public string? TemplateId { get; init; }
    public IDictionary<string, object> TemplateData { get; init; } = new Dictionary<string, object>();
    public string? IdempotencyKey { get; init; }
    public DateTimeOffset? SendAt { get; init; }
}
```

### EmailAttachment

Represents file attachments with support for inline content and streaming.

```csharp
public enum EmailAttachmentDisposition { Attachment, Inline }

public sealed record EmailAttachment
{
    public required string FileName { get; init; }
    public byte[]? Content { get; init; }
    public Stream? ContentStream { get; init; }
    public required string ContentType { get; init; }
    public EmailAttachmentDisposition Disposition { get; init; } = EmailAttachmentDisposition.Attachment;
    public string? ContentId { get; init; }
}
```

### EmailPriority

Specifies the priority of an email.

```csharp
public enum EmailPriority { Low = 1, Normal = 2, High = 3 }
```

## Usage

This package contains only abstractions and models. To send emails, you'll need a concrete implementation package such as:

- `Cirreum.Communications.Email.SendGrid` - SendGrid email provider implementation

### Dependency Injection

Register your chosen email provider in your application startup:

```csharp
// Example with SendGrid provider
builder.AddSendGridEmailClient("Default", settings =>
{
    settings.ApiKey = configuration["SendGrid:ApiKey"]!;
    settings.DefaultFrom = new EmailAddress("noreply@company.com", "Company Notifications");
});
```

### Basic Usage

```csharp
public class NotificationService
{
    private readonly IEmailService _emailService;

    public NotificationService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendWelcomeEmail(string emailAddress, string userName)
    {
        var message = new EmailMessage
        {
            From = new EmailAddress("noreply@company.com", "Company"),
            To = { new EmailAddress(emailAddress, userName) },
            Subject = "Welcome to Our Service",
            HtmlContent = $"<h1>Welcome {userName}!</h1><p>Thanks for joining us.</p>",
            TextContent = $"Welcome {userName}! Thanks for joining us."
        };

        var result = await _emailService.SendEmailAsync(message);
    }
}
```

### Bulk Template Email

```csharp
public async Task SendNewsletterAsync(List<string> subscribers)
{
    var template = new EmailMessage
    {
        From = new EmailAddress("newsletter@company.com", "Company Newsletter"),
        Subject = "Monthly Newsletter - {{month}}",
        TemplateKey = "Newsletter",
        TemplateData = new Dictionary<string, object>
        {
            ["month"] = DateTime.Now.ToString("MMMM yyyy")
        }
    };

    var response = await _emailService.SendBulkEmailAsync(
        template,
        subscribers.Select(s => new EmailAddress(s)).ToList());
}
```

### Personalized Bulk Email

```csharp
public async Task SendPersonalizedEmails(List<User> users)
{
    var messages = users.Select(user => new EmailMessage
    {
        From = new EmailAddress("support@company.com", "Support"),
        To = { new EmailAddress(user.Email, user.Name) },
        Subject = $"Personal update for {user.Name}",
        HtmlContent = $"<h1>Hi {user.Name}!</h1><p>Your account balance: ${user.Balance:F2}</p>",
        TemplateData = new Dictionary<string, object>
        {
            ["userName"] = user.Name,
            ["balance"] = user.Balance
        }
    });

    var response = await _emailService.SendBulkEmailAsync(messages);
}
```

### Email with Attachments

```csharp
public async Task SendEmailWithAttachment(string recipient, byte[] pdfContent)
{
    var message = new EmailMessage
    {
        From = new EmailAddress("reports@company.com", "Reports"),
        To = { new EmailAddress(recipient) },
        Subject = "Your Monthly Report",
        HtmlContent = "<p>Please find your monthly report attached.</p>",
        Attachments =
        {
            new EmailAttachment
            {
                FileName = "monthly-report.pdf",
                Content = pdfContent,
                ContentType = "application/pdf"
            }
        }
    };

    var result = await _emailService.SendEmailAsync(message);
}
```

### Validation Mode

```csharp
var response = await _emailService.SendBulkEmailAsync(
    template: welcomeTemplate,
    recipients: emailAddresses.Select(a => new EmailAddress(a)),
    validateOnly: true);

var validEmails = response.Results.Where(r => r.Success).Select(r => r.EmailAddress);
```

## Features

- **Multiple content types**: Support for both HTML and plain text content
- **Rich recipients**: Full `EmailAddress` model with display names for To, Cc, and Bcc
- **File attachments**: Byte[] or Stream, with inline vs. attachment disposition
- **Template system**: Provider-agnostic templates via `TemplateKey` or direct `TemplateId`
- **Bulk operations**: Efficient batch sending with per-recipient results
- **Validation mode**: Test email formatting and templates without sending
- **Custom headers & metadata**: Categories, headers, custom args, reply-to
- **Priority levels**: Standard priority mapping (Low, Normal, High)
- **Idempotency & scheduling**: Support for deduplication and scheduled sends
- **Detailed results**: Provider info, status codes, retry-after, validation errors
- **Provider agnostic**: Works with any email provider implementation

## Provider Implementations

This package provides the abstractions only. Choose from available provider implementations:

- **SendGrid**: `Cirreum.Communications.Email.SendGrid`
- Additional providers can be added by implementing the `IEmailService` interface

## Contributing

This package is part of the Cirreum ecosystem. Follow the established patterns when contributing new features or provider implementations.







