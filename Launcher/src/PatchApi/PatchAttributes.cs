using JetBrains.Annotations;

namespace PatchApi;

/// <summary>
/// Patch manager will neither instantiate nor run this patch.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class)]
public sealed class IgnorePatchAttribute : Attribute;