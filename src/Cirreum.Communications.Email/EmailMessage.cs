namespace Cirreum.Communications.Email;

/// <summary>
/// Represents a fully formed, provider-agnostic email message.
/// </summary>
public sealed record EmailMessage {

	/// <summary>
	/// Required sender.
	/// </summary>
	public required EmailAddress From { get; init; }

	/// <summary>
	/// Optional reply-to. If null, implementations may fall back to <see cref="From"/>.
	/// </summary>
	public EmailAddress? ReplyTo { get; init; }

	/// <summary>
	/// Primary recipients. Must contain at least one item.
	/// </summary>
	public IList<EmailAddress> To { get; init; } = [];

	/// <summary>
	/// Carbon copy recipients.
	/// </summary>
	public IList<EmailAddress> Cc { get; init; } = [];

	/// <summary>
	/// Blind carbon copy recipients.
	/// </summary>
	public IList<EmailAddress> Bcc { get; init; } = [];

	/// <summary>
	/// Subject line (required unless template fully renders subject).
	/// </summary>
	public string? Subject { get; init; }

	/// <summary>
	/// Plaintext content. At least one of <see cref="TextContent"/> or <see cref="HtmlContent"/> must be provided (unless a template generates both).
	/// </summary>
	public string? TextContent { get; init; }

	/// <summary>
	/// HTML content. At least one of <see cref="TextContent"/> or <see cref="HtmlContent"/> must be provided (unless a template generates both).
	/// </summary>
	public string? HtmlContent { get; init; }

	/// <summary>
	/// Optional headers (e.g., X-Custom-Header). Implementations should handle case-insensitivity.
	/// </summary>
	public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

	/// <summary>
	/// Optional tags/categories for analytics/segmentation.
	/// </summary>
	public IList<string> Categories { get; init; } = [];

	/// <summary>
	/// Provider-agnostic custom properties for webhook Cirreumelation, metadata, etc.
	/// </summary>
	public IDictionary<string, string> CustomArgs { get; init; } = new Dictionary<string, string>();

	/// <summary>
	/// Message priority. Implementations may map to headers like X-Priority/Importance.
	/// </summary>
	public EmailPriority Priority { get; init; } = EmailPriority.Normal;

	/// <summary>
	/// Optional logical template key (your abstraction), which can be mapped to a provider-specific template id in the concrete implementation.
	/// </summary>
	public string? TemplateKey { get; init; }

	/// <summary>
	/// Optional provider-specific template identifier. Prefer <see cref="TemplateKey"/> in application code.
	/// </summary>
	public string? TemplateId { get; init; }

	/// <summary>
	/// Named values used when rendering a template.
	/// </summary>
	public IDictionary<string, object> TemplateData { get; init; } = new Dictionary<string, object>();

	/// <summary>
	/// Attachments (inline or attachment disposition supported).
	/// </summary>
	public IList<EmailAttachment> Attachments { get; init; } = [];

	/// <summary>
	/// Optional unique key used to deduplicate retry attempts across providers.
	/// </summary>
	public string? IdempotencyKey { get; init; }

	/// <summary>
	/// Optional scheduled send time. If in the past, implementations should send immediately.
	/// </summary>
	public DateTimeOffset? SendAt { get; init; }

}