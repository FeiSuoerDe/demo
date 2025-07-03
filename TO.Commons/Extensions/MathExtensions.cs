using Godot;

namespace TO.Commons.Extensions;

public static class MathExtensions
{
    public static float RemapRange(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        var normalized = Mathf.InverseLerp(fromMin, fromMax, value);
        return Mathf.Lerp(toMin, toMax, normalized);
    }

}