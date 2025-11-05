namespace Cirreum.Communications.Email;
/// <summary>
/// Aggregated response for bulk email operations.
/// </summary>
public sealed record EmailResponse(int Sent, int Failed, IReadOnlyList<EmailResult> Results);