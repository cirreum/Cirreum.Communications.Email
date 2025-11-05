namespace Cirreum.Communications.Email;

/// <summary>
/// Represents an email attachment.
/// </summary>
public sealed record EmailAttachment {

	/// <summary>
	/// Attachment filename.
	/// </summary>
	public required string FileName { get; init; }

	/// <summary>
	/// File content as bytes. (If both Content and ContentStream are null, implementation should throw during validation.)
	/// </summary>
	public byte[]? Content { get; init; }

	/// <summary>
	/// Optional source stream for large attachments. Implementations should read/rewind as needed.
	/// </summary>
	public Stream? ContentStream { get; init; }

	/// <summary>
	/// MIME content type.
	/// </summary>
	public required string ContentType { get; init; }

	/// <summary>
	/// Attachment or Inline.
	/// </summary>
	public EmailAttachmentDisposition Disposition { get; init; } = EmailAttachmentDisposition.Attachment;

	/// <summary>
	/// Content-ID for inline attachments (e.g., images referenced by &lt;img src="cid:..."/&gt;).
	/// </summary>
	public string? ContentId { get; init; }

}