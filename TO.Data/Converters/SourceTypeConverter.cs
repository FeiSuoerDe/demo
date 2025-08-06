using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TO.Commons.Enums.Game;

namespace TO.Data.Converters;

public class SourceTypeConverter : ValueConverter<SourceType, string>
{
    public SourceTypeConverter() : base(
        v => v.ToString(),
        v => (SourceType)Enum.Parse(typeof(SourceType), v))
    {
    }
}
