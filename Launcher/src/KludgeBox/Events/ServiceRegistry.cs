﻿#region

using KludgeBox.Core;

#endregion

namespace KludgeBox.Events;

public class ServiceRegistry
{
    public List<object> Services { get; private set; } = new();

    public void Register(object service)
    {
        Log.Info($"Registering service {service.GetType().FullName}");
        Services.Add(service);
    }

    public void RegisterServices()
    {
        var types = ReflectionExtensions.FindTypesWithAttributes(typeof(GameServiceAttribute));
        foreach (Type type in types)
        {
            try
            {
                var instance = type.GetInstanceOfType();
                Register(instance);
            }
            catch (Exception e)
            {
                Log.Error($"Can't instantiate service {type.FullName}:\n{e.Message}");
            }
        }
    }
}

public class GameServiceAttribute : Attribute
{
    public ListenerSide Side { get; set; }
    
    public GameServiceAttribute(ListenerSide side = ListenerSide.Both)
    {
        Side = side;
    }
}