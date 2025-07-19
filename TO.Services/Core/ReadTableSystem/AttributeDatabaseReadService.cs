using Godot;
using TO.Data.Database;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Services.Abstractions.Core.ReadTableSystem;
using TO.Repositories.Abstractions.Core.ReadTableSystem;

namespace TO.Services.Core.ReadTableSystem;

public class AttributeDatabaseReadService(IAttributeSetCacheRepo cache) : IAttributeDatabaseReadService
{
    public AttributeSet GetAttributeSetById(string id)
    {
        var cachedSet = cache.GetAttributeSet(id);
        if (cachedSet != null)
        {
            var cachedAttributeSet = new AttributeSet(cachedSet.Attributes);
            return cachedAttributeSet;
        }

        using var context = new AttributeDatabaseContext();
       
        var attributeSetEntity = context.AttributeSets.FirstOrDefault(a => a.Id == id);
        if (attributeSetEntity == null) return null!;

        List<AttributeValue> attributes = [];

        if (attributeSetEntity.Id.Contains("player"))
        {
            var basicValues = context.BasicAttributeValues.Where(v => v.AttributeSetId == id).ToList();
            GD.Print(basicValues.Count);
            attributes.AddRange(basicValues.Select(val =>
                new AttributeValue(val.AttributeType, val.MaxValue, val.MinValue, val.MaxValue)));
        }
        else if (attributeSetEntity.Id.Contains("ship"))
        {
            var shipValues = context.ShipAttributeValues.Where(v => v.AttributeSetId == id).ToList();
            attributes.AddRange(shipValues.Select(val =>
                new AttributeValue(val.AttributeType, val.MaxValue, val.MinValue, val.MaxValue)));
        }

        var attributeSet = new AttributeSet(attributes);
        cache.CacheAttributeSet(id,attributeSet);
        return attributeSet;
    }
}