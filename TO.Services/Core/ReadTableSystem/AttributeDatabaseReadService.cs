using Godot;
using TO.Data.Database;
using TO.Data.Factories;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Services.Abstractions.Core.ReadTableSystem;
using TO.Repositories.Abstractions.Core.ReadTableSystem;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TO.Data.DTO.GameAbilitySystem.GameplayAttribute;
using TO.Data.Registries;

namespace TO.Services.Core.ReadTableSystem;

public class AttributeDatabaseReadService(IAttributeSetCacheRepo cache) : IAttributeDatabaseReadService
{
    public AttributeSet GetAttributeSetById(string id)
    {
        var cachedSet = cache.GetAttributeSet(id);
        if (cachedSet != null)
        {
            return new AttributeSet(cachedSet.Attributes);
        }

        using var context = new AttributeDatabaseContext();

        // Per user feedback, we are now abandoning the relational query on AttributeSets
        // and instead querying AttributeValues directly. This gives us more control and
        // bypasses the part of EF Core that was failing.
        var valueDtos = context.AttributeValues
            .Include(v => v.AttributeDefinition) // A simpler, more direct include.
            .Where(v => v.AttributeSetId == id)
            .ToList();

        if (valueDtos.Count == 0)
        {
            // Per user instruction, the AttributeSets table is for design-time only.
            // If no values are found for a given id, we can treat the set as non-existent for runtime.
            return null!;
        }

        var attributes = new List<AttributeValue>();
        foreach (var valueDto in valueDtos)
        {
            if (valueDto.AttributeDefinition == null) continue; // Skip if the definition wasn't loaded.

            var definition = AttributeRegistry.Get(valueDto.AttributeDefinition.Id);
            if (definition == null) continue;
            
            attributes.Add(AttributeValueFactory.Create(definition, valueDto.BaseValue, valueDto.MinValue, valueDto.MaxValue));
        }

        var attributeSet = new AttributeSet(attributes);
        cache.CacheAttributeSet(id, attributeSet);
        return attributeSet;
    }
}
