using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TO.Commons.Enums.Game;

namespace TO.Data.Converters;

public class ModifierOperationTypeConverter : ValueConverter<ModifierOperationType, string>
{
    public ModifierOperationTypeConverter() : base(
        v => v.ToString(),
        v => Enum.Parse<ModifierOperationType>(v))
    {
    }
}