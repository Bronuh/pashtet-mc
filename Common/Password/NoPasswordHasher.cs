namespace Common.Password;

/// <summary>
/// Класс-заглушка, которая никак не хеширует и не шифрует пароль.
/// </summary>
public class NoPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return password;
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        return password == storedHash;
    }
}