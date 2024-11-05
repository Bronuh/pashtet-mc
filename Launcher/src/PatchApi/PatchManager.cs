#region

using System.Linq;
using System.Reflection;
using KludgeBox.Collections;
using KludgeBox.Core;
using KludgeBox.Events.Global;
using PatchApi.Events;

#endregion

namespace PatchApi;

public class PatchManager
{
    public bool Sealed { get; private set; }
    public ReadOnlyHashSet<LauncherPatch> Patches => _patches.AsReadOnly();
    private HashSet<LauncherPatch> _patches = new ();

    public bool RegisterPatch(LauncherPatch patch)
    {
        if(Sealed)
            throw new InvalidOperationException("Регистрация патчей после закрытия хранилища невозможно.");
        
        var added = _patches.All(p => p.GetType() != patch.GetType()) && _patches.Add(patch);

        return added;
    }

    public void RunPatches()
    {
        Sealed = true;
        var orderedPatches = _patches.OrderByDescending(p => p.Priority);
        foreach (var patch in orderedPatches)
        {
            try
            {
                if (patch.CanRun())
                {
                    if (!EventBus.PublishIsCancelled(new PatchAboutToRunEnvent(patch)))
                    {
                        patch.ProcessAndRun();
                    }
                }
            }
            catch (Exception e)
            {
                EventBus.Publish(new PatchRunErroredEvent(patch, e));
                Log.Error(e);
            }
        }
    }

    public void LoadPatches(Assembly assembly)
    {
        var patches = assembly.FindAllTypesThatDeriveFrom<LauncherPatch>();
        var nonAbstractPatches = patches.Where(t => !t.IsAbstract && t.HasParameterlessConstructor());

        var types = assembly.GetTypes().ToList();
        EventBus.RegisterListeners(types);

        foreach (var patchType in nonAbstractPatches)
        {
            if (patchType.HasAttribute<IgnorePatchAttribute>())
            {
                Log.Info($"Проигнорирован патч: {patchType.FullName}");
                continue;
            }

            try
            {
                var patch = Activator.CreateInstance(patchType) as LauncherPatch;
                if (!RegisterPatch(patch))
                {
                    Log.Warning($"Попытка зарегистрировать уже зарегистрированный патч: {patchType.FullName}");
                }
                else
                {
                    Log.Info($"Зарегистрирован патч: {patchType.FullName}");
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}