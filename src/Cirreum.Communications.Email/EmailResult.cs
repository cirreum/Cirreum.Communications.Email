namespace Cirreum.Communications.Email;

/// <summary>
/// Result for an individual email operation (single or per-recipient in bulk).
/// </summary>
public sealed record EmailResult {

	/// <summary>
	/// The recipient address this result pertains to (primary To).
	/// </summary>
	public required string EmailAddress { get; init; }

	/// <summary>
	/// True if the provider accepted the message for delivery (or validation succeeded when applicable).
	/// </summary>
	public required bool Success { get; init; }

	/// <summary>
	/// Provider-generated message id, if available.
	/// </summary>
	public string? MessageId { get; init; }

	/// <summary>
	/// Human-readable error message, if any.
	/// </summary>
	public string? ErrorMessage { get; init; }

	/// <summary>
	/// Provider identifier (e.g., "SendGrid").
	/// </summary>
	public string? Provider { get; init; }

	/// <summary>
	/// HTTP status code or provider-specific numeric code (when known).
	/// </summary>
	public int? StatusCode { get; init; }

	/// <summary>
	/// Suggested retry-after duration for throttling (e.g., when StatusCode == 429).
	/// </summary>
	public TimeSpan? RetryAfter { get; init; }

	/// <summary>
	/// Cirreumelation id for cross-system tracing.
	/// </summary>
	public string? CirreumelationId { get; init; }

	/// <summary>
	/// Validation errors captured when validate-only flows are used, or when provider rejects before enqueue.
	/// </summary>
	public IList<string> ValidationErrors { get; init; } = [];

}