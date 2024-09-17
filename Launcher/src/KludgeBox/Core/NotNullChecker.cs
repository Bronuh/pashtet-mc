#region

using System.Reflection;

#endregion

namespace KludgeBox.Core;

public static class NotNullChecker
{
    public static void CheckProperties(object obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        Type type = obj.GetType();
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        foreach (PropertyInfo property in type.GetProperties(bindingFlags))
        {
            bool isNull = property.GetValue(obj) == null;
            bool hasNotNullAttribute = Attribute.IsDefined(property, typeof(NotNullAttribute));
            if (hasNotNullAttribute && isNull)
            {
                Log.Critical($"Property '{property.Name}' is null, but has NotNull attribute in type {obj.GetType()}.");
            }
        }
    }
}