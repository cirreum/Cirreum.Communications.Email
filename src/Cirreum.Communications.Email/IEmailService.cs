namespace Cirreum.Communications.Email;

/// <summary>
/// Defines the contract for an Email Service.
/// </summary>
public interface IEmailService {

	/// <summary>
	/// Sends a single email.
	/// Implementations should validate <see cref="EmailMessage"/> (non-empty To, content/template presence) before sending.
	/// </summary>
	/// <param name="message">The email message to send containing recipient, subject, content, and other email properties.</param>
	/// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
	/// <returns>A task that represents the asynchronous send operation. The task result contains an <see cref="EmailResult"/> with send status and any error details.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
	/// <exception cref="InvalidOperationException">Thrown when the email message fails validation (e.g., missing recipients or content).</exception>
	Task<EmailResult> SendEmailAsync(
		EmailMessage message,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends bulk emails to multiple recipients using a shared template/message shape.
	/// The <paramref name="template"/> should include From, Subject, TemplateKey/Id and TemplateData shared across recipients.
	/// Implementations MAY fan out in batches and return per-recipient results.
	/// </summary>
	/// <param name="template">The email template containing shared properties like From, Subject, and template configuration.</param>
	/// <param name="recipients">The collection of email addresses to send the templated message to.</param>
	/// <param name="validateOnly">If true, only validates the email configuration without actually sending. Defaults to false.</param>
	/// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
	/// <returns>A task that represents the asynchronous bulk send operation. The task result contains an <see cref="EmailResponse"/> with overall status and per-recipient results.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="template"/> or <paramref name="recipients"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="recipients"/> is empty or contains invalid email addresses.</exception>
	Task<EmailResponse> SendBulkEmailAsync(
		EmailMessage template,
		IEnumerable<EmailAddress> recipients,
		bool validateOnly = false,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends bulk emails with per-recipient personalization (fully-formed <see cref="EmailMessage"/> objects).
	/// Implementations MAY chunk and parallelize subject to provider rate limits, returning per-recipient results.
	/// </summary>
	/// <param name="messages">The collection of fully-formed email messages to send, each with its own recipients and content.</param>
	/// <param name="validateOnly">If true, only validates the email messages without actually sending. Defaults to false.</param>
	/// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
	/// <returns>A task that represents the asynchronous bulk send operation. The task result contains an <see cref="EmailResponse"/> with overall status and per-message results.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="messages"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="messages"/> is empty or contains invalid email messages.</exception>
	Task<EmailResponse> SendBulkEmailAsync(
		IEnumerable<EmailMessage> messages,
		bool validateOnly = false,
		CancellationToken cancellationToken = default);

}