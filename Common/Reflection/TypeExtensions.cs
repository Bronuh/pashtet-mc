#region

using System.Reflection;

#endregion

namespace Common.Reflection;

public static class TypeExtensions
{
    /// <summary>
    /// Try to find methods with all the attributes provided in the attributes param.
    /// </summary>
    /// <param name="type">The type to search in.</param>
    /// <param name="attributes">The attributes that the methods must have.</param>
    /// <returns>The MethodInfo array containing methods that have all the specified attributes.</returns>
    public static MethodInfo[] FindMethodsWithAttributes(this Type type, params Type[] attributes)
    {
        return type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                               BindingFlags.NonPublic)
            .Where(m => attributes.All(attr => m.GetCustomAttributes(attr, true).Length > 0))
            .ToArray();
    }
}