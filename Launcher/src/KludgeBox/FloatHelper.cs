#region

#if GODOT_REAL_T_IS_DOUBLE
global using real = double;
global using altreal = float;
#else
global using real = float;
global using altreal = double;
#endif

#endregion

namespace KludgeBox;

public static class FloatHelper
{
#if GODOT_REAL_T_IS_DOUBLE
    public const bool RealIsDouble = true;
#else
    public const bool RealIsDouble = false;
#endif
}