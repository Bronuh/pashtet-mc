#region

using KludgeBox.SaveLoad.SaveLoadTypes;
using Newtonsoft.Json.Linq;

#endregion

namespace KludgeBox.SaveLoad
{
	public class Saver
	{
		public bool IsWorking => (CurrentObject is not null) && (SaveLoad.Mode is SaveLoadMode.Saving);

		public JObject RootObject { get; private set; } = null; // Root object that must be serialized
		public JObject RefsObject { get; private set; } = null; // Contains a list of references
		public JObject DataObject { get; private set; } = null; // Contains a main structure of data

		public JObject CurrentObject { get; private set; } = null; // Currently processing object

		public string SavingResult { get; private set; }

		public void InitSave()
		{
			SaveLoad.Mode = SaveLoadMode.Saving;
			RootObject = new JObject();
			CurrentObject = RootObject;

			if (EnterObject(SaveLoad.DataObjectName))
			{
				DataObject = CurrentObject;
				ExitObject();
			}
		}

        internal void SaveRefs()
        {
            CurrentObject = RootObject;
            if (EnterObject(SaveLoad.RefsObjectName))
            {
                RefsObject = CurrentObject;
                ExitObject();
            }
            SaveLoad_Reference.LinkDict();
        }
        

		public void FinalizeSave()
		{
			SavingResult = CurrentObject?.ToString();
		}


		// Enters the object. Creates it if necessary.
		// Returns true if entered the object and false if not.
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

			// And then, if there is no existing property, create a new property with a new object
			var newObject = new JProperty(name, new JObject());
			CurrentObject.Add(newObject);
			CurrentObject = newObject.Value as JObject;

			return true;
		}

		// Exits the current object and enters it's parent.
		internal void ExitObject()
		{
			if (!IsWorking)
				return;

			CurrentObject = (JObject)CurrentObject.Parent.Parent;
		}

		// Writes the final string to the property
		internal void WriteProperty(string name, string value)
		{
			if (!IsWorking)
				return;

			CurrentObject.Add(new JProperty(name, value));
		}

		internal void Stop()
		{
			FinalizeSave();
			CurrentObject = null;
		}
	}
}
