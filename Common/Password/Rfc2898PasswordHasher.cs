#region

using System.Security.Cryptography;

#endregion

namespace Common.Password;

public sealed class Rfc2898PasswordHasher : IPasswordHasher
{
	// Constants for hash size, salt size, and iterations
	public int SaltSize = 16; // 128-bit salt
	public int HashSize = 32; // 256-bit hash
	public int Iterations = 10000; // Number of iterations for PBKDF2

	/// <summary>
	/// Hashes a password using PBKDF2 with a random salt.
	/// </summary>
	/// <param name="password">The password to hash.</param>
	/// <returns>A hashed password string containing the salt and hash.</returns>
	public string HashPassword(string password)
	{
		// Generate a random salt
		byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);//new byte[SaltSize];

		// Hash the password using PBKDF2
		var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
		byte[] hash = pbkdf2.GetBytes(HashSize);

		// Combine salt and hash to store them together
		byte[] hashBytes = new byte[SaltSize + HashSize];
		Array.Copy(salt, 0, hashBytes, 0, SaltSize);
		Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

		// Convert to base64 for storage
		return Convert.ToBase64String(hashBytes);
	}

	/// <summary>
	/// Verifies a password against a stored hash.
	/// </summary>
	/// <param name="password">The password to verify.</param>
	/// <param name="storedHash">The stored hash (salt + hash) to compare against.</param>
	/// <returns>True if the password is valid, false otherwise.</returns>
	public bool VerifyPassword(string password, string storedHash)
	{
		// Extract the bytes from the stored hash
		byte[] hashBytes = Convert.FromBase64String(storedHash);

		// Extract the salt (first part of the byte array)
		byte[] salt = new byte[SaltSize];
		Array.Copy(hashBytes, 0, salt, 0, SaltSize);

		// Hash the password with the extracted salt
		var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
		byte[] hash = pbkdf2.GetBytes(HashSize);

		// Compare the new hash with the stored hash (second part of the byte array)
		for (int i = 0; i < HashSize; i++)
		{
			if (hashBytes[SaltSize + i] != hash[i])
			{
				return false;
			}
		}

		return true;
	}
}