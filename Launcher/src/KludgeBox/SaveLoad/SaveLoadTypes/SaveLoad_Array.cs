namespace KludgeBox.SaveLoad.SaveLoadTypes
{
	public static class SaveLoad_Array
	{
		public static void Link<TValue>(ref List<TValue> array, string label, LinkMode defaultLinkMode = LinkMode.Undefined) 
		{

            LinkMode linkMode;
            Type elementType = typeof(TValue);
            if (defaultLinkMode == LinkMode.Undefined)
                linkMode = TypeResolver.ResolveLinkType(elementType);
            else
                linkMode = defaultLinkMode;

			if(SaveLoad.Mode is SaveLoadMode.ScanRefs)
			{
				foreach (var item in array)
					if (item is IReferencable rItem)
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
					SaveLoad.Saver.WriteProperty("arrayLength", array.Count.ToString());

					// Loop through the array elements and save them individually.
					for (int i = 0; i < array.Count; i++)
					{
						// Create a label for the array element based on the index.
						string elementLabel = $"element{i}";

						// Link the array element using the appropriate method based on its type.
						TValue value = array[i];
						switch (linkMode)
						{
							case LinkMode.Value:
								SaveLoad_Value.Link(ref value, elementLabel);
								break;
							case LinkMode.Exposable:
								var exposable = value as IExposable;
								SaveLoad_Exposable.Link(ref exposable, elementLabel);
								break;
                            case LinkMode.Reference:
                                var referencable = value as IReferencable;
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
				var prop = SaveLoad.Loader.CurrentObject[label];

				if (Parser.IsNull(label))
				{
					array = null;
					Log.Warning($"Unable to load array for '{label}' in {SaveLoad.Loader.CurrentObject.Path}");
					return;
				}

				// Get the length of the array from the saved data.
				int arrayLength = int.Parse((string)SaveLoad.Loader.CurrentObject[label]["arrayLength"]);

				// Create a new array with the retrieved length.
				array = new List<TValue>(arrayLength);

				// Enter the object with the specified label for loading array data.
				if (SaveLoad.EnterObject(label))
				{
					// Loop through the array elements and load them individually.
					for (int i = 0; i < arrayLength; i++)
					{
						if(linkMode is not LinkMode.Reference)
						{
                            // Create a label for the array element based on the index.
                            string elementLabel = $"element{i}";

                            // Link the array element using the appropriate method based on its type.
                            TValue value = default(TValue);
                            switch (linkMode)
                            {
                                case LinkMode.Value:
                                    SaveLoad_Value.Link(ref value, elementLabel);
                                    break;
                                case LinkMode.Exposable:
                                    var exposable = value as IExposable;
                                    SaveLoad_Exposable.Link(ref exposable, elementLabel);
                                    value = (TValue)exposable;
                                    break;
                                case LinkMode.Reference:
                                    var reference = value as IReferencable;
                                    SaveLoad_Reference.Link(ref reference, elementLabel);
                                    value = (TValue)reference;
                                    break;
                                default:
                                    // Handle undefined LinkMode for the array element type.
                                    break;

                            }
                            array[i] = value;
						}
					}

					// Exit the object after loading the array data.
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
                            if (key == "arrayLength")
                                continue;

                            TValue val = default;
                            var reference = value as IReferencable;
                            SaveLoad_Reference.Link(ref reference, key);
                            val = (TValue)reference;
							array.Add(val);
                        }
                        SaveLoad.ExitObject();
                    }
                }

                if (typeof(TValue).IsAssignableTo(typeof(IExposable)))
				{
					foreach (var value in array)
					{
						var exposable = value as IExposable;
						exposable.ExposeData();
					}
				}
			}
		}
	}
}
