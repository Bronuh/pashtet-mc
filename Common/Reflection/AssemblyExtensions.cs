using System.Reflection;
using System.Runtime.Loader;

namespace Common.Reflection;

public static class AssemblyExtensions
{
    public static AssemblyLoadContext? GetCurrentLoadContext()
    {
        return AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
    }
    
    public static AssemblyLoadContext? GetLoadContext(Assembly assembly)
    {
        return AssemblyLoadContext.GetLoadContext(assembly);
    }

    public static IEnumerable<Type> GetAllTypes(IEnumerable<Assembly>? assemblies = null)
    {
        assemblies ??= GetAllAssemblies();
        return assemblies.SelectMany(x => x.GetTypes());
    }
    
    public static Assembly[] GetAllAssemblies()
    {
        return AppDomain.CurrentDomain.GetAssemblies();
    }
    
    /// <summary>
    /// Try to find a type by its full or short name.
    /// </summary>
    /// <param name="typeName">The short or full name of the type.</param>
    /// <returns>The found type or null if not found.</returns>
    public static Type? FindTypeByName(string typeName)
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
    public static TType? GetInstanceOfType<TType>(string typeName) where TType : class
    {
        Type? type = FindTypeByName(typeName);
        if (type is not null && typeof(TType).IsAssignableFrom((Type)type))
        {
            return Activator.CreateInstance(type) as TType;
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
    /// Try to find types with all the attributes provided in the attributes param.
    /// </summary>
    /// <param name="attributes">One or more attribute types to search for.</param>
    /// <returns>An array of types that have all the specified attributes.</returns>
    public static Type[] FindTypesWithAttributes(params Type[] attributes)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => attributes.All(attr => t.GetCustomAttribute(attr) != null))
            .ToArray();
    }

    /// <summary>
    /// Try to find types with all the attributes provided in the attributes param within a specific assembly.
    /// </summary>
    /// <param name="assembly">The assembly to search in.</param>
    /// <param name="attributes">One or more attribute types to search for.</param>
    /// <returns>An array of types that have all the specified attributes.</returns>
    public static Type[] FindTypesWithAttributes(this Assembly assembly, params Type[] attributes)
    {
        return assembly.GetTypes()
            .Where(t => attributes.All(attr => t.GetCustomAttribute(attr) != null))
            .ToArray();
    }

    /// <summary>
    /// Try to find types with all the attributes provided in the attributes param within multiple assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to search in.</param>
    /// <param name="attributes">One or more attribute types to search for.</param>
    /// <returns>An array of types that have all the specified attributes.</returns>
    public static Type[] FindTypesWithAttributes(this IEnumerable<Assembly> assemblies, params Type[] attributes)
    {
        return assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => attributes.All(attr => t.GetCustomAttribute(attr) != null))
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
    public static object? CreateInstance(this Type type)
    {
        if (type.HasParameterlessConstructor())
        {
            return Activator.CreateInstance(type);
        }

        var firstAvailableConstructor = type.GetConstructors()[0];
        var neededParams = firstAvailableConstructor.GetParameters();
        List<object?> args = new();
        
        foreach (var param in neededParams)
        {
            try
            {
                args.Add(Activator.CreateInstance(param.ParameterType));
            }
            catch(Exception e)
            {
                args.Add(null);
            }
        }
            
        var instance = Activator.CreateInstance(type, args.ToArray());
		
        return instance;
    }
}