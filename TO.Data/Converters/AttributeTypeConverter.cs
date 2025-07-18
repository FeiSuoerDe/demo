using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TO.Commons.Enums.Game;

namespace TO.Data.Converters;

public class AttributeTypeConverter : ValueConverter<AttributeType, string>
{
    public AttributeTypeConverter() : base(
        v => v.ToString(),
        v => Enum.Parse<AttributeType>(v))
    {
    }
}
