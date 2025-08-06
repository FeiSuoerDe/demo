using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Repositories.Abstractions.Core.GameAbilitySystem;

/// <summary>
/// 属性集仓储接口
/// </summary>
public interface IAttributeSetRepo
{
    /// <summary>
    /// 添加属性集
    /// </summary>
    /// <param name="attributeSet">属性集</param>
    void Add(AttributeSet attributeSet);
    
    /// <summary>
    /// 移除属性集
    /// </summary>
    /// <param name="id">属性集ID</param>
    /// <returns>是否成功移除</returns>
    bool Remove(Guid id);
    
    /// <summary>
    /// 根据ID获取属性集
    /// </summary>
    /// <param name="id">属性集ID</param>
    /// <returns>属性集，如果不存在返回null</returns>
    AttributeSet? GetById(Guid id);
    
    /// <summary>
    /// 获取所有属性集
    /// </summary>
    /// <returns>所有属性集</returns>
    IEnumerable<AttributeSet> GetAll();
    
    /// <summary>
    /// 检查指定ID的属性集是否存在
    /// </summary>
    /// <param name="id">属性集ID</param>
    /// <returns>如果存在返回true，否则返回false</returns>
    bool Exists(Guid id);
    
    // Removed: IEnumerable<AttributeSet> FindWithEffect(Guid effectId);
    
    /// <summary>
    /// 清空所有属性集
    /// </summary>
    void Clear();
    
    /// <summary>
    /// 属性集数量
    /// </summary>
    int Count { get; }
}