namespace Cirreum.Communications.Email;

using System.Net.Mail;

/// <summary>
/// Simple value object for an email address with optional display name.
/// </summary>
public readonly record struct EmailAddress(string Address, string? Name = null) {

	/// <summary>
	/// Returns a string representation of the object, displaying the name and address if both are available,  or just the
	/// address if the name is not specified.
	/// </summary>
	/// <returns>A string in the format "Name &lt;Address&gt;" if the name is not null or whitespace;  otherwise, the address.</returns>
	public override string ToString() => string.IsNullOrWhiteSpace(this.Name) ? this.Address : $"{this.Name} <{this.Address}>";

	/// <summary>
	/// Determines whether the specified string is a valid email address.
	/// </summary>
	/// <remarks>
	/// An email address is considered valid if it is not null, not empty, and conforms to the standard
	/// email address format.
	/// </remarks>
	/// <param name="email">The email address to validate. This can be null or empty.</param>
	/// <returns><see langword="true"/> if the specified string is a valid email address; otherwise, <see langword="false"/>.</returns>
	public static bool IsValidEmailAddress(string? email) {

		if (string.IsNullOrWhiteSpace(email)) {
			return false;
		}

		try {
			var mailAddress = new MailAddress(email);
			return mailAddress.Address == email;
		} catch {
			return false;
		}

	}
}