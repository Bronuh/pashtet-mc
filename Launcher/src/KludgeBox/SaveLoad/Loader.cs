#region

using Newtonsoft.Json.Linq;

#endregion

namespace KludgeBox.SaveLoad
{
	public class Loader
	{
		public bool IsWorking => (CurrentObject is not null) && ((SaveLoad.Mode is SaveLoadMode.Loading) || (SaveLoad.Mode is SaveLoadMode.PostLoading));


		public JObject RootObject { get; private set; } = null; // Root object that must be serialized
		public JObject RefsObject { get; private set; } = null; // Contains a list of references
		public JObject DataObject { get; private set; } = null; // Contains a main structure of data

		public JObject CurrentObject { get; internal set; } = null; // Currently processing object

		public void InitLoad(string input)
		{
			SaveLoad.Mode = SaveLoadMode.Loading;
			CurrentObject = JObject.Parse(input);
			RootObject = CurrentObject;

			if (EnterObject(SaveLoad.RefsObjectName))
			{
				RefsObject = CurrentObject;
				ExitObject();
			}
		}

		internal void LoadData()
		{
			CurrentObject = RootObject;
			if (EnterObject(SaveLoad.DataObjectName))
			{
				DataObject = CurrentObject;
				ExitObject();
			}
		}

		/// <summary>
		/// Enters the object
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal bool EnterObject(string name)
		{
			if (!IsWorking)
				return false;

			// First, check if the property exists
			if (CurrentObject.ContainsKey(name))
			{
				// Try to get property value as JObject
				var property = CurrentObject[name] as JObject;

				// If we expecting an JObject but getting something else, the we can't enter object
				if (property is null)
					return false;

				// Otherwise everything is ok and we can now use the object
				CurrentObject = property;
				return true;
			}

			return false;
		}

		internal void ExitObject()
		{
			if (!IsWorking)
				return;

			CurrentObject = (JObject)CurrentObject.Parent.Parent;
		}

		internal void Stop()
		{
			CurrentObject = null;
		}
	}
}
