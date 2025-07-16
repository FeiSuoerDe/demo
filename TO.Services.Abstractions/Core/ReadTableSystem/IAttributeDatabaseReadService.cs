using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Services.Abstractions.Core.ReadTableSystem;

public interface IAttributeDatabaseReadService
{
    AttributeSet GetAttributeSetById(string id);
}