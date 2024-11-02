namespace KludgeBox.SaveLoad.SaveLoadTypes
{
    public static class SaveLoad_Reference
    {
        public static Dictionary<string, IReferencable> _knownReferences = new Dictionary<string, IReferencable>();
        private static Dictionary<IReferencable, string> _refIds = new Dictionary<IReferencable, string>();

        public static void Link<TValue>(ref TValue target, string label) where TValue : class, IReferencable
        {
            if(SaveLoad.Mode is SaveLoadMode.ScanRefs)
            {
                if (target is null)
                    return;
                if(RegisterReferencable(target))
                    target.ExposeData();
            }

            if (SaveLoad.Mode is SaveLoadMode.Saving)
            {
                // Saving mode - check if the target is null and handle accordingly.
                // Null values are saved as an object with single "isNull" property
                if (target is null)
                {
                    // The target is null, so write a property indicating it's null and exit the object.
                    if (SaveLoad.EnterObject(label))
                    {
                        SaveLoad.Saver.WriteProperty("isNull", "true");
                        SaveLoad.ExitObject();
                        return;
                    }
                }

                if (SaveLoad.EnterObject(label))
                {
                    // Write the actual type name of the target object to be used during loading.
                    SaveLoad.Saver.WriteProperty("referenceId", target.GetReferenceId());
                    SaveLoad.ExitObject();
                    return;
                }
            }

            // At this step we collecting refIds for later restoration on SaveLoadMode.PostLoading
            if (SaveLoad.Mode is SaveLoadMode.Loading)
            {
                if (target is null)
                    return;

                if (!_refIds.ContainsKey(target))
                {
                    if (SaveLoad.EnterObject(label))
                    {
                        string refId = SaveLoad.CurrentObject["referenceId"].ToString();
                        _refIds[target] = refId;
                        SaveLoad.ExitObject();
                        return;
                    }
                }
            }

            if (SaveLoad.Mode is SaveLoadMode.PostLoading)
            {
                if (SaveLoad.EnterObject(label))
                {
                    string refId = SaveLoad.CurrentObject["referenceId"].ToString();
                    SaveLoad.ExitObject();
                    target = _knownReferences[refId] as TValue;
                }
            }
        }

        public static void LinkDict()
        {
            SaveLoad_Dictionary.Link(ref _knownReferences, SaveLoad.RefsObjectName, LinkMode.Exposable);
        }

        public static void ResolveLinks()
        {
            foreach (var pair in _knownReferences)
            {
                if (SaveLoad.EnterObject(pair.Key))
                {
                    pair.Value.ExposeData();
                    SaveLoad.ExitObject();
                }
            }
        }
        internal static bool RegisterReferencable(IReferencable referencable)
        {
            string id = referencable.GetReferenceId();
            if(!_knownReferences.ContainsKey(id) || !_refIds.ContainsKey(referencable))
            {
                _knownReferences[id] = referencable;
                _refIds[referencable] = id;
                return true;
            }
            return false;
        }

        internal static void Clear()
        {
            _knownReferences.Clear();
            _refIds.Clear();
        }

        internal static void DoNothing()
        {
            Log.Debug("");
        }
    }
}
