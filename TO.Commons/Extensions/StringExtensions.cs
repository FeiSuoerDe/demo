using Godot;
using Godot.Collections;

namespace TO.Commons.Extensions;

public static class StringExtensions
{
    public static Array<Vector2I> ParseVector2IArray(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return [];
            
        try 
        {
            var elements = input.Split(';')
                .Select(s => s.Trim().Trim('(', ')'))
                .Select(s => s.Split(','))
                .Where(parts => parts.Length == 2)
                .Select(parts => new Vector2I(
                    int.Parse(parts[0].Trim()),
                    int.Parse(parts[1].Trim()))).ToList()
                ;
            return new Array<Vector2I>(elements);
        }
        catch
        {
            return [];
        }
    }

    public static Vector2I ParseVector2I(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Vector2I.Zero;
            
        try 
        {
            var parts = input.Trim().Trim('(', ')').Split(',');
            if (parts.Length == 2)
            {
                return new Vector2I(
                    int.Parse(parts[0].Trim()),
                    int.Parse(parts[1].Trim()));
            }
            return Vector2I.Zero;
        }
        catch
        {
            return Vector2I.Zero;
        }
    }
}