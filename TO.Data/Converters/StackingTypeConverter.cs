using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TO.Commons.Enums.Game;

namespace TO.Data.Converters;

public class StackingTypeConverter : ValueConverter<EffectStackingType, string>
{
    public StackingTypeConverter() : base(
        v => v.ToString(),
        v => Enum.Parse<EffectStackingType>(v))
    {
    }
}