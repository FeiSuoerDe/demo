using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TO.Commons.Enums.Game;

namespace TO.Data.Converters;

public class EffectTypeConverter : ValueConverter<EffectType, string>
{
    public EffectTypeConverter() : base(
        v => v.ToString(),
        v => Enum.Parse<EffectType>(v))
    {
    }
}