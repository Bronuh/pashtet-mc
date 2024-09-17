#region

using KludgeBox.Core;

#endregion

namespace KludgeBox.SaveLoad
{
	/// <summary>
	/// Provides methods for saving and loading data of classes that implement the IExposable interface.
	/// </summary>
	public class SaveLoad_Exposable
	{
		/// <summary>
		/// Links the target object to the specified label for saving and loading.
		/// </summary>
		/// <typeparam name="TValue">The type of the target object.</typeparam>
		/// <param name="target">The target object to be linked.</param>
		/// <param name="label">The label associated with the target object for saving and loading.</param>
		/// <exception cref="ArgumentNullException">Thrown when the target is null.</exception>
		public static void Link<TValue>(ref TValue target, string label) where TValue : class, IExposable
		{

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

				// Enter the object with the specified label for saving data.
				if (SaveLoad.EnterObject(label))
				{
					// Write the actual type name of the target object to be used during loading.
					SaveLoad.Saver.WriteProperty(SaveLoad.TypePropertyName, target.GetTypeName());
					// Call the ExposeData method of the target object to recursively save its data.
					target.ExposeData();
					// Exit the object after saving its data.
					SaveLoad.ExitObject();
					return;
				}
			}

			if (SaveLoad.Mode is SaveLoadMode.Loading)
			{
				// Loading mode - retrieve the property of the target object with the specified label.
				var prop = SaveLoad.Loader.CurrentObject[label];

				// Check if the property is null, indicating the target object was not saved.
				if (Parser.IsNull(label))
				{
					// Set the target to null value and display a warning message.
					target = null;
					Log.Warning($"Unable to load exposable for '{label}' in {SaveLoad.Loader.CurrentObject.Path}");
					return;
				}

				// Enter the object with the specified label for loading data.
				if (SaveLoad.EnterObject(label))
				{
					// Retrieve the type name of the target object from the loaded data.
					var typeName = (string)SaveLoad.CurrentObject[SaveLoad.TypePropertyName];
					// Create an instance of the target object based on the retrieved type name.
					IExposable exposable = ReflectionExtensions.GetInstanceOfType<IExposable>(typeName);
					// Call the ExposeData method of the created object to load its data.
					exposable.ExposeData();
					// Convert the loaded object to the target type and assign it to the target.
					target = (TValue)exposable;
					// Exit the object after loading its data.
					SaveLoad.ExitObject();
				}
			}

			if (SaveLoad.Mode is SaveLoadMode.PostLoading)
			{
				// Post-loading mode - call the ExposeData method of the target object 
				// to run data restoration and processing inside ExposeData method.
				target?.ExposeData();
			}
		}
	}
}
