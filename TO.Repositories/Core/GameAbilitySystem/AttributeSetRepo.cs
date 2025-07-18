
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Repositories.Bases;
using TO.Repositories.Abstractions.Core.GameAbilitySystem;

namespace TO.Repositories.Core.GameAbilitySystem;

/// <summary>
/// 属性集仓储
/// </summary>
public class AttributeSetRepo : BaseRepo, IAttributeSetRepo
{
    private readonly Dictionary<Guid, AttributeSet> _attributeSets;
    private readonly Lock _lock = new Lock();
    
    public AttributeSetRepo()
    {
        _attributeSets = new Dictionary<Guid, AttributeSet>();
    }
    
    /// <summary>
    /// 添加属性集
    /// </summary>
    /// <param name="attributeSet">属性集</param>
    public void Add(AttributeSet attributeSet)
    {
        if (attributeSet == null)
            throw new ArgumentNullException(nameof(attributeSet));
            
        lock (_lock)
        {
            _attributeSets[attributeSet.Id] = attributeSet;
        }
    }
    
    /// <summary>
    /// 移除属性集
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <returns>是否成功移除</returns>
    public bool Remove(Guid attributeSetId)
    {
        lock (_lock)
        {
            return _attributeSets.Remove(attributeSetId);
        }
    }
    
    /// <summary>
    /// 获取属性集
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <returns>属性集，如果不存在返回null</returns>
    public AttributeSet? GetById(Guid attributeSetId)
    {
        lock (_lock)
        {
            _attributeSets.TryGetValue(attributeSetId, out var attributeSet);
            return attributeSet;
        }
    }
    
    /// <summary>
    /// 获取所有属性集
    /// </summary>
    /// <returns>属性集列表</returns>
    public IEnumerable<AttributeSet> GetAll()
    {
        lock (_lock)
        {
            return _attributeSets.Values.ToList();
        }
    }
    
    /// <summary>
    /// 检查属性集是否存在
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <returns>是否存在</returns>
    public bool Exists(Guid attributeSetId)
    {
        lock (_lock)
        {
            return _attributeSets.ContainsKey(attributeSetId);
        }
    }
    
    /// <summary>
    /// 获取属性集数量
    /// </summary>
    public int Count => _attributeSets.Count;
    
    /// <summary>
    /// 清空所有属性集
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _attributeSets.Clear();
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing)
        {
            lock (_lock)
            {
                _attributeSets.Clear();
            }
        }
        
        base.Dispose(disposing);
    }
}