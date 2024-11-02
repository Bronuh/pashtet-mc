namespace KludgeBox.SaveLoad.SaveLoadTypes
{
	/// <summary>
	/// Provides methods for saving and loading value types.
	/// </summary>
	public static class SaveLoad_Value
	{
		/// <summary>
		/// Links the value to the specified label for saving and loading.
		/// </summary>
		/// <typeparam name="TValue">The type of the value to be linked.</typeparam>
		/// <param name="value">The value to be linked.</param>
		/// <param name="label">The label associated with the value for saving and loading.</param>
		public static void Link<TValue>(ref TValue value, string label)
		{
			// Saving process
			if(SaveLoad.Mode is SaveLoadMode.Saving)
			{
				// Check if the value is null and handle accordingly.
				if (value is null)
				{
					// The value is null, so write a property indicating it's null and exit the object.
					if (SaveLoad.EnterObject(label))
					{
						SaveLoad.Saver.WriteProperty("isNull","true");
						SaveLoad.ExitObject();
						return;
					}
				}
				// Convert the value to a string representation using GD.VarToStr and Variant.
				SaveLoad.Saver.WriteProperty(label, value.ToString());
				return;
			}

			// Loading process
			if(SaveLoad.Mode is SaveLoadMode.Loading)
			{
				// Check if the property is null or the value cannot be parsed to the specified type.
				if (Parser.IsNull(label) || !Parser.CanParse(typeof(TValue)))
				{
					// Set the value to default and display a warning message.
					value = default;
					Log.Warning($"Unable to parse value for '{label}' in {SaveLoad.Loader.CurrentObject.Path}");
					return;
				}
				// Parse the string representation of the value back to the specified type.
				value = Parser.Parse<TValue>((string)SaveLoad.Loader.CurrentObject[label]);
			}
		}
	}
}
