using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TO.Commons.Enums.Game;

namespace TO.Data.Converters;

public class TagsConverter : ValueConverter<HashSet<EffectTags>, string>
{
    public TagsConverter() : base(
        v => string.Join(",", v.Select(t => t.ToString())),
        v => string.IsNullOrEmpty(v) ? new HashSet<EffectTags>() : new HashSet<EffectTags>(v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => Enum.Parse<EffectTags>(s.Trim()))))
    {
    }
}