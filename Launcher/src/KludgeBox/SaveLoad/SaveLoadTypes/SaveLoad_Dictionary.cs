namespace KludgeBox.SaveLoad.SaveLoadTypes
{
	public static class SaveLoad_Dictionary
	{
		public static void Link<TValue>(ref Dictionary<string, TValue> array, string label, LinkMode defaultLinkMode = LinkMode.Undefined)
		{
            LinkMode linkMode;
            Type elementType = typeof(TValue);
            if (defaultLinkMode == LinkMode.Undefined)
                linkMode = TypeResolver.ResolveLinkType(elementType);
            else
                linkMode = defaultLinkMode;

            if (SaveLoad.Mode is SaveLoadMode.ScanRefs)
            {
                foreach (var item in array)
                    if (item.Value is IReferencable rItem)
                        SaveLoad_Reference.Link(ref rItem, "ref");
            }

            // Saving process
            if (SaveLoad.Mode is SaveLoadMode.Saving)
			{
				if (array is null)
				{
					if (SaveLoad.EnterObject(label))
					{
						SaveLoad.Saver.WriteProperty("isNull", "true");
						SaveLoad.ExitObject();
						return;
					}
				}

				// Enter the object with the specified label for saving array data.
				if (SaveLoad.EnterObject(label))
				{
					SaveLoad.Saver.WriteProperty("dictLength", array.Count.ToString());
					// Loop through the array elements and save them individually.
					foreach (var keyValue in array)
					{
						string elementLabel = keyValue.Key;
                        var val = keyValue.Value; // wtf, C#?

						switch (linkMode)
						{
							case LinkMode.Value:
								SaveLoad_Value.Link(ref val, elementLabel);
								break;
							case LinkMode.Exposable:
								var exposable = val as IExposable;
								SaveLoad_Exposable.Link(ref exposable, elementLabel);
								break;
                            case LinkMode.Reference:
                                var referencable = val as IReferencable;
                                SaveLoad_Reference.Link(ref referencable, elementLabel);
                                break;
                            default:
								// Handle undefined LinkMode for the array element type.
								break;
						}
					}

					// Exit the object after saving the array data.
					SaveLoad.ExitObject();
					return;
				}
			}

			// Loading process
			if (SaveLoad.Mode is SaveLoadMode.Loading)
			{
				if (Parser.IsNull(label))
				{
					array = null;
					Log.Warning($"Unable to load array for '{label}' in {SaveLoad.Loader.CurrentObject.Path}");
					return;
				}

				// Get the length of the array from the saved data.
				int arrayLength = int.Parse((string)SaveLoad.Loader.CurrentObject[label]["dictLength"]);
				// Create a new array with the retrieved length.
				if (SaveLoad.EnterObject(label))
				{
					array = new Dictionary<string, TValue>();

                    foreach ((var key, var value) in SaveLoad.Loader.CurrentObject)
					{
						if (key=="dictLength")
							continue;
						
						TValue val = default;
						switch (linkMode)
						{
							case LinkMode.Value:
								SaveLoad_Value.Link(ref val, key);
								break;
							case LinkMode.Exposable:
								var exposable = val as IExposable;
								SaveLoad_Exposable.Link(ref exposable, key);
								val = (TValue)exposable;
								break;
                            case LinkMode.Reference:
                                var reference = val as IReferencable;
                                SaveLoad_Reference.Link(ref reference, key);
                                val = (TValue)reference;
                                break;
                            default:
								// Handle undefined LinkMode for the array element type.
								break;
						}

						array[key] = val;
					}
					SaveLoad.ExitObject();
				}
			}

			if (SaveLoad.Mode is SaveLoadMode.PostLoading)
			{
				if (linkMode == LinkMode.Reference)
				{
					if (SaveLoad.EnterObject(label))
					{
						foreach ((var key, var value) in SaveLoad.Loader.CurrentObject)
						{
							if (key == "dictLength")
								continue;

							TValue val = default;
							var reference = value as IReferencable;
							SaveLoad_Reference.Link(ref reference, key);
							val = (TValue)reference;
							array[key] = val;
						}
						SaveLoad.ExitObject();
					}
                }
                if (typeof(TValue).IsAssignableTo(typeof(IExposable)))
                {
					foreach ((var key, var value) in array)
					{
						var exposable = value as IExposable;
						exposable.ExposeData();
					}
				}
			}
		}
	}
}
