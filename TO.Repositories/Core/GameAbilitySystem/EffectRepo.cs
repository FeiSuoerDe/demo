using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using TO.Repositories.Bases;
using TO.Repositories.Abstractions.Core.GameAbilitySystem;

namespace TO.Repositories.Core.GameAbilitySystem;

/// <summary>
/// 效果仓储
/// </summary>
public class EffectRepo : BaseRepo, IEffectRepo
{
    private readonly List<AttributeEffect> _globalEffects = [];
    private readonly Lock _lock = new Lock();

    /// <summary>
    /// 获取全局效果数量
    /// </summary>
    public int GlobalEffectCount
    {
        get
        {
            lock (_lock)
            {
                return _globalEffects.Count;
            }
        }
    }
    
    /// <summary>
    /// 添加全局效果
    /// </summary>
    /// <param name="effect">效果</param>
    public void AddGlobalEffect(AttributeEffect effect)
    {
        if (effect == null)
            throw new ArgumentNullException(nameof(effect));
            
        lock (_lock)
        {
            _globalEffects.Add(effect);
        }
    }
    
    /// <summary>
    /// 移除全局效果
    /// </summary>
    /// <param name="effectId">效果ID</param>
    /// <returns>被移除的效果，如果不存在返回null</returns>
    public AttributeEffect? RemoveGlobalEffect(Guid effectId)
    {
        lock (_lock)
        {
            var effect = _globalEffects.FirstOrDefault(e => e?.Id == effectId);
            if (effect != null)
            {
                _globalEffects.Remove(effect);
            }
            return effect;
        }
    }
    
    /// <summary>
    /// 获取所有全局效果
    /// </summary>
    /// <returns>全局效果列表</returns>
    public IEnumerable<AttributeEffect> GetAllGlobalEffects()
    {
        lock (_lock)
        {
            return _globalEffects.Where(e => e != null).Cast<AttributeEffect>().ToList();
        }
    }
    
    /// <summary>
    /// 查找具有指定标签的全局效果
    /// </summary>
    /// <param name="tag">效果标签</param>
    /// <returns>具有该标签的效果列表</returns>
    public IEnumerable<AttributeEffect> FindGlobalEffectsWithTag(EffectTags tag)
    {
        // lock (_lock)
        // {
        //     return _globalEffects
        //         .Where(e => e != null && e.Tags.TryGetValue(tag, out var value)))
        //         .Cast<AttributeEffect>()
        //         .ToList();
        // }
        return null;
    }
    
    /// <summary>
    /// 获取过期的全局效果
    /// </summary>
    /// <returns>过期的全局效果列表</returns>
    public IEnumerable<AttributeEffect> GetExpiredGlobalEffects()
    {
        lock (_lock)
        {
            return _globalEffects
                .Where(e => e != null && e.IsExpired)
                .Cast<AttributeEffect>()
                .ToList();
        }
    }
    
    /// <summary>
    /// 移除已过期的全局效果
    /// </summary>
    /// <returns>被移除的过期效果列表</returns>
    public IEnumerable<AttributeEffect> RemoveExpiredGlobalEffects()
    {
        lock (_lock)
        {
            var expiredEffects = _globalEffects
                .Where(e => e != null && e.IsExpired)
                .Cast<AttributeEffect>()
                .ToList();
                
            foreach (var effect in expiredEffects)
            {
                _globalEffects.Remove(effect);
            }
            
            return expiredEffects;
        }
    }
    
    /// <summary>
    /// 清理过期的全局效果
    /// </summary>
    /// <returns>清理的效果数量</returns>
    public int CleanupExpiredGlobalEffects()
    {
        lock (_lock)
        {
            var expiredEffects = _globalEffects.Where(e => e != null && e.IsExpired).ToList();
            foreach (var effect in expiredEffects)
            {
                _globalEffects.Remove(effect);
            }
            return expiredEffects.Count;
        }
    }
    
    /// <summary>
    /// 清空所有全局效果
    /// </summary>
    public void ClearGlobalEffects()
    {
        lock (_lock)
        {
            _globalEffects.Clear();
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing)
        {
            lock (_lock)
            {
                _globalEffects.Clear();
            }
        }
        
        base.Dispose(disposing);
    }
}