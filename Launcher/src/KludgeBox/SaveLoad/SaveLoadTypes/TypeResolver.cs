namespace KludgeBox.SaveLoad.SaveLoadTypes
{
	/// <summary>
	/// Enum representing the preferred property processing method for a type link.
	/// </summary>
	public enum LinkMode
	{
		/// <summary>
		/// Indicates that the type link's processing method is unknown or undefined.
		/// </summary>
		Undefined,

		/// <summary>
		/// Indicates that the type link must be processed as a value type (SaveLoad_Value).
		/// </summary>
		Value,

		/// <summary>
		/// Indicates that the type link must be processed as an IExposable object (SaveLoad_Exposable).
		/// </summary>
		Exposable,

		Reference
	}

	/// <summary>
	/// A static utility class for resolving the preferred property processing method for a given Type.
	/// </summary>
	public static class TypeResolver
	{
		/// <summary>
		/// Resolves the preferred property processing method for a given Type.
		/// </summary>
		/// <param name="type">The Type to be processed.</param>
		/// <returns>The preferred LinkMode for the given Type.</returns>
		public static LinkMode ResolveLinkType(Type type)
		{
			// Check if the type can be parsed using the Parser class.
			if (Parser.CanParse(type))
			{
				return LinkMode.Value; // If the type can be parsed, it is considered a value type.
			}

            // Check if the type is assignable to the IReferencable interface.
            if (type.IsAssignableTo(typeof(IReferencable)))
            {
                return LinkMode.Reference; // If the type is assignable to IReferencable, it should be processed as an IReferencable object.
            }

            // Check if the type is assignable to the IExposable interface.
            if (type.IsAssignableTo(typeof(IExposable)))
			{
				return LinkMode.Exposable; // If the type is assignable to IExposable, it should be processed as an IExposable object.
			}

			return LinkMode.Undefined; // If the type doesn't match any of the known processing methods, it is considered undefined.
		}
	}
}
