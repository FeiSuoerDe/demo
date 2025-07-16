using Godot;
using TO.Data.Database;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Services.Abstractions.Core.ReadTableSystem;
using TO.Repositories.Abstractions.Core.ReadTableSystem;

namespace TO.Services.Core.ReadTableSystem;

public class AttributeDatabaseReadService : IAttributeDatabaseReadService
{
    private readonly IAttributeSetCacheRepo _cache;

    public AttributeDatabaseReadService(IAttributeSetCacheRepo cache)
    {
        _cache = cache;
    }

    public AttributeSet GetAttributeSetById(string id)
    {
        var cachedSet = _cache.GetAttributeSet(id);
        if (cachedSet != null)
        {
            return cachedSet;
        }

        using var context = new AttributeDatabaseContext();
       
        var attributeSetEntity = context.AttributeSets.FirstOrDefault(a => a.Id == id);
        if (attributeSetEntity == null) return null!;

        List<AttributeValue> attributes = [];

        if (attributeSetEntity.Id.Contains("basic"))
        {
            var basicValues = context.BasicAttributeValues.Where(v => v.AttributeSetId == id).ToList();
            GD.Print(basicValues.Count);
            attributes.AddRange(basicValues.Select(val =>
                new AttributeValue(val.AttributeType, val.BaseValue, val.MinValue, val.MaxValue)));
        }
        else if (attributeSetEntity.Id.Contains("ship"))
        {
            var shipValues = context.ShipAttributeValues.Where(v => v.AttributeSetId == id).ToList();
            attributes.AddRange(shipValues.Select(val =>
                new AttributeValue(val.AttributeType, val.BaseValue, val.MinValue, val.MaxValue)));
        }

        var attributeSet = new AttributeSet(attributes);
        _cache.CacheAttributeSet(id,attributeSet);
        return attributeSet;
    }
}