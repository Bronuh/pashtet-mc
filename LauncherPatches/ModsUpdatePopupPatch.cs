using KludgeBox;
using KludgeBox.Events.EventTypes;
using KludgeBox.Events.Global;
using PatchApi;
using PatchApi.Events;

namespace LauncherPatches;

public class ModsUpdatePopupPatch : LauncherPatch
{
    protected override void Run()
    {
        Log.Info("Применен патч подсчета изменений при обновлении");
        EventBus.Subscribe<PopupRequestEnqueueEvent>(OnPopupRequestEnqueueEvent);
    }
    
    private static void OnPopupRequestEnqueueEvent(PopupRequestEnqueueEvent evt)
    {
        var request = evt.Request;
    }
}