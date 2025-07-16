using TO.Commons.Enums.UI;
using TO.Repositories.Core.SceneSystem.Effects;
using TO.Repositories.Core.SceneSystem.Effects.Bases;

namespace TO.Repositories.Core.SceneSystem
{
    /// <summary>
    /// 过渡效果注册表
    /// </summary>
    public static class TransitionEffectRegistry
    {
        private static readonly Dictionary<TransitionEffectType, Func<bool, ITransitionEffect>> _effectFactories = new();
        
        static TransitionEffectRegistry()
        {
            // 注册内置效果
            RegisterEffect(TransitionEffectType.Fade, isEntering => new FadeTransitionEffect(isEntering));
        }
        
        /// <summary>
        /// 注册过渡效果工厂方法（枚举版本）
        /// </summary>
        /// <param name="effectType">效果类型</param>
        /// <param name="factory">工厂方法</param>
        public static void RegisterEffect(TransitionEffectType effectType, Func<bool, ITransitionEffect> factory)
        {
            _effectFactories[effectType] = factory;
        }
        

        
        /// <summary>
    /// 创建指定类型的过渡效果
    /// </summary>
    /// <param name="effectType">效果类型</param>
    /// <param name="isEntering">是否为入场效果</param>
    /// <returns>过渡效果实例，如果类型未注册则返回null</returns>
    public static ITransitionEffect? CreateEffect(TransitionEffectType effectType, bool isEntering)
    {
        return _effectFactories.TryGetValue(effectType, out var factory) ? factory(isEntering) : null;
    }
        

        
        /// <summary>
        /// 检查效果类型是否已注册（枚举版本）
        /// </summary>
        /// <param name="effectType">效果类型</param>
        /// <returns>是否已注册</returns>
        public static bool IsRegistered(TransitionEffectType effectType)
        {
            return _effectFactories.ContainsKey(effectType);
        }
        

        
        /// <summary>
        /// 获取所有已注册的效果类型（枚举版本）
        /// </summary>
        /// <returns>已注册的效果类型集合</returns>
        public static IEnumerable<TransitionEffectType> GetRegisteredTypes()
        {
            return _effectFactories.Keys;
        }
        

    }
}