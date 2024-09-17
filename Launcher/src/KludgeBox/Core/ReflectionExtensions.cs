#region

using System.Linq;
using System.Reflection;

#endregion

namespace KludgeBox.Core;

public static class ReflectionExtensions
{
	/// <summary>
	/// Get the main executing assembly.
	/// </summary>
	/// <returns>The current executing assembly.</returns>
	public static Assembly GetCurrentAssembly()
	{
		return Assembly.GetEntryAssembly();
	}

	/// <summary>
	/// Get all the loaded assemblies.
	/// </summary>
	/// <returns>An array of all loaded assemblies.</returns>
	public static Assembly[] GetAllAssemblies()
	{
		return AppDomain.CurrentDomain.GetAssemblies();
	}

	/// <summary>
	/// Try to find a type by its full or short name.
	/// </summary>
	/// <param name="typeName">The short or full name of the type.</param>
	/// <returns>The found type or null if not found.</returns>
	public static Type FindTypeByName(string typeName)
	{
		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetTypes())
			.FirstOrDefault(t => t.FullName == typeName || t.Name == typeName);
	}

	/// <summary>
	/// Try to find a type implementing IExposable by its name and instantiate it.
	/// </summary>
	/// <param name="typeName">The name of the type implementing IExposable.</param>
	/// <returns>An instance of the type implementing IExposable or null if not found.</returns>
	public static TType GetInstanceOfType<TType>(string typeName) where TType : class
	{
		Type type = FindTypeByName(typeName);
		if (type != null && typeof(TType).IsAssignableFrom((Type)type))
		{
			return (TType)Activator.CreateInstance(type);
		}

		return null;
	}
	
	/// <summary>
	/// Try to find a type implementing IExposable by its name and instantiate it.
	/// </summary>
	/// <param name="typeName">The name of the type implementing IExposable.</param>
	/// <returns>An instance of the type implementing IExposable or null if not found.</returns>
	public static object GetInstanceOfType(this Type type)
	{
		return Activator.CreateInstance(type);
	}

	/// <summary>
	/// Returns the full name of the object's type.
	/// </summary>
	/// <param name="obj">The object whose type name to retrieve.</param>
	/// <returns>The full name of the object's type.</returns>
	public static string GetTypeName(this object obj)
	{
		return obj?.GetType()?.FullName;
	}

	/// <summary>
	/// Try to find types with all of the attributes provided in the attributes param.
	/// </summary>
	/// <param name="attributes">One or more attribute types to search for.</param>
	/// <returns>An array of types that have all of the specified attributes.</returns>
	public static Type[] FindTypesWithAttributes(params Type[] attributes)
	{
		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetTypes())
			.Where(t => attributes.All(attr => t.GetCustomAttribute(attr) != null))
			.ToArray();
	}

	/// <summary>
	/// Try to find types with all of the attributes provided in the attributes param within a specific assembly.
	/// </summary>
	/// <param name="assembly">The assembly to search in.</param>
	/// <param name="attributes">One or more attribute types to search for.</param>
	/// <returns>An array of types that have all of the specified attributes.</returns>
	public static Type[] FindTypesWithAttributes(this Assembly assembly, params Type[] attributes)
	{
		return assembly.GetTypes()
			.Where(t => attributes.All(attr => t.GetCustomAttribute(attr) != null))
			.ToArray();
	}

	/// <summary>
	/// Try to find types with all of the attributes provided in the attributes param within multiple assemblies.
	/// </summary>
	/// <param name="assemblies">The assemblies to search in.</param>
	/// <param name="attributes">One or more attribute types to search for.</param>
	/// <returns>An array of types that have all of the specified attributes.</returns>
	public static Type[] FindTypesWithAttributes(this IEnumerable<Assembly> assemblies, params Type[] attributes)
	{
		return assemblies
			.SelectMany(a => a.GetTypes())
			.Where(t => attributes.All(attr => t.GetCustomAttribute(attr) != null))
			.ToArray();
	}

	/// <summary>
	/// Try to find methods with all of the attributes provided in the attributes param.
	/// </summary>
	/// <param name="type">The type to search in.</param>
	/// <param name="attributes">The attributes that the methods must have.</param>
	/// <returns>The MethodInfo array containing methods that have all of the specified attributes.</returns>
	public static MethodInfo[] FindMethodsWithAttributes(this Type type, params Type[] attributes)
	{
		return type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
		                       BindingFlags.NonPublic)
			.Where(m => attributes.All(attr => m.GetCustomAttributes(attr, true).Length > 0))
			.ToArray();
	}


	/// <summary>
	/// Finds all types that derive from the specified base type.
	/// </summary>
	/// <typeparam name="TBase">The base type.</typeparam>
	/// <returns>An enumerable collection of types that derive from the specified base type.</returns>
	public static IEnumerable<Type> FindAllTypesThatDeriveFrom<TBase>()
	{
		return GetAllAssemblies().SelectMany(a => a.GetTypes()).Where(type => type.IsSubclassOf(typeof(TBase)));
	}


	/// <summary>
	/// Finds all types that derive from the specified base type.
	/// </summary>
	/// <typeparam name="TBase">The base type.</typeparam>
	/// <returns>An enumerable collection of types that derive from the specified base type.</returns>
	public static IEnumerable<Type> FindAllTypesThatDeriveFrom<TBase>(this Assembly assembly)
	{
		return assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(TBase)));
	}

	/// <summary>
	/// Returns true if the type has the specified attribute.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="attributeType"></param>
	/// <returns></returns>
	public static bool HasAttribute(this MemberInfo type, Type attributeType)
	{
		return type.GetCustomAttribute(attributeType) is not null;
	}
	
	/// <summary>
	/// Returns true if the type has the specified attribute.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="attributeType"></param>
	/// <returns></returns>
	public static bool HasAttribute<TAttribute>(this MemberInfo type) where TAttribute : Attribute
	{
		return type.HasAttribute(typeof(TAttribute));
	}
	
	/// <summary>
	/// Returns value of field or property.
	/// </summary>
	/// <param name="instance"></param>
	/// <param name="member"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public static object GetValue(this MemberInfo member, object instance)
	{
		if (member is FieldInfo fieldInfo)
			return fieldInfo.GetValue(instance);
        
		if (member is PropertyInfo propertyInfo)
			if (propertyInfo.CanRead && propertyInfo.CanWrite)
				return propertyInfo.GetValue(instance);
			else 
				throw new Exception($"PropertyInfo {propertyInfo} has no getter or setter.");
        
		var type = member.GetType();
		throw new Exception($"MemberInfo {member} is {type} and neither a FieldInfo nor a PropertyInfo.");
	}

	/// <summary>
	/// Assigns value to field or property.
	/// </summary>
	/// <param name="instance"></param>
	/// <param name="member"></param>
	/// <param name="value"></param>
	/// <exception cref="Exception"></exception>
	public static void SetValue(this MemberInfo member, object instance, object value)
	{
		if (member is FieldInfo fieldInfo)
		{
			fieldInfo.SetValue(instance, value);
			return;
		}
        
		if (member is PropertyInfo propertyInfo)
		{
			if (propertyInfo.CanRead && propertyInfo.CanWrite)
			{
				propertyInfo.SetValue(instance, value);
				return;
			}
			else
			{
				throw new Exception($"PropertyInfo {propertyInfo} has no getter or setter.");
			}
		}

		var type = member.GetType();
		throw new Exception($"MemberInfo {member} is {type} and neither a FieldInfo nor a PropertyInfo.");
	}

	/// <summary>
	/// Will return type of field or property
	/// </summary>
	/// <remarks>
	/// Note, that this method is used for serialization and deserialization, so it will throw if property has no getter or setter.
	/// </remarks>
	/// <param name="member"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public static Type GetMemberType(this MemberInfo member)
	{
		if (member is FieldInfo fieldInfo)
			return fieldInfo.FieldType;
        
		if (member is PropertyInfo propertyInfo)
			if (propertyInfo.CanRead && propertyInfo.CanWrite)
				return propertyInfo.PropertyType;
			else 
				throw new Exception($"PropertyInfo {propertyInfo} has no getter or setter.");
        
		throw new Exception($"MemberInfo {member} is neither a FieldInfo nor a PropertyInfo.");
	}
	
	/// <summary>
	/// Whether the type has a parameterless constructor
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public static bool HasParameterlessConstructor(this Type type)
	{
		return type.GetConstructor(Type.EmptyTypes) != null;
	}
	
	/// <summary>
	/// tries to create an instance of the type, even if there is no parameterless constructor.
	/// </summary>
	/// <returns></returns>
	public static object CreateInstance(this Type type)
	{
		if (type.HasParameterlessConstructor())
		{
			return Activator.CreateInstance(type);
		}

		var firstAvailableConstructor = type.GetConstructors()[0];
		var neededParams = firstAvailableConstructor.GetParameters();
		List<object> args = new();
        
		foreach (var param in neededParams)
		{
			try
			{
				args.Add(Activator.CreateInstance(param.ParameterType));
			}
			catch(Exception e)
			{
				Log.Error(e);
				args.Add(null);
			}
		}
            
		var instance = Activator.CreateInstance(type, args.ToArray());
		
		return instance;
	}
	
	/// <summary>
	/// Will return all public instance properties with both public getter and setter and fields without <see cref="NonSerializedAttribute"/>
	/// </summary>
	/// <param name="fieldsOnly">Properties will not be included if this is set to true.</param>
	/// <returns></returns>
	public static List<MemberInfo> GetSerializableMembers(this Type type, bool fieldsOnly = false)
	{
		var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
		IEnumerable<MemberInfo> properties = new List<MemberInfo>();
		if (!fieldsOnly)
		{
			var allPublicProperties = type.GetProperties(bindingFlags);
			properties = allPublicProperties
				.Where(p => p.CanRead && p.CanWrite);
		}

		var allPublicfields = type.GetFields(bindingFlags);
		var fields = allPublicfields.Where(field => !field.HasAttribute<NonSerializedAttribute>());
		
		return properties
			.Concat(fields)
			.ToList();
	}
}