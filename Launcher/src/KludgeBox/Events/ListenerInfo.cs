#region

using System.Reflection;
using KludgeBox.Events.EventTypes;

#endregion

namespace KludgeBox.Events;

public readonly record struct ListenerInfo<T>(Action<T> Action, bool IsDefault, MethodInfo Source) where T : IEvent;