namespace Common.Password;

/// <summary>
/// Represents an algorithm for hashing and verifying a password.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Returns the hash of the input password.
    /// </summary>
    /// <param name="password">The password to be hashed.</param>
    /// <returns>The hash of the password.</returns>
    string HashPassword(string password);
    
    /// <summary>
    /// Verifies if the password matches the hash.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="storedHash">The hash.</param>
    /// <returns></returns>
    bool VerifyPassword(string password, string storedHash);
}