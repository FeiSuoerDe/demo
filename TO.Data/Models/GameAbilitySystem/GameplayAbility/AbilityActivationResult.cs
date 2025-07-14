using System;
using System.Collections.Generic;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Data.Models.GameAbilitySystem.GameplayAbility
{
    /// <summary>
    /// 技能激活结果
    /// </summary>
    public class AbilityActivationResult
    {
        /// <summary>
        /// 是否成功激活
        /// </summary>
        public bool IsSuccess { get; private set; }
        
        /// <summary>
        /// 结果消息
        /// </summary>
        public string Message { get; private set; }
        
        /// <summary>
        /// 产生的效果列表
        /// </summary>
        public List<AttributeEffect> GeneratedEffects { get; private set; }
        
        /// <summary>
        /// 造成的伤害值
        /// </summary>
        public float Damage { get; private set; }
        
        /// <summary>
        /// 治疗值
        /// </summary>
        public float Healing { get; private set; }
        
        /// <summary>
        /// 技能执行时间
        /// </summary>
        public DateTime ExecutionTime { get; private set; }
        
        /// <summary>
        /// 额外数据
        /// </summary>
        public Dictionary<string, object> ExtraData { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="message">结果消息</param>
        /// <param name="damage">伤害值</param>
        /// <param name="healing">治疗值</param>
        public AbilityActivationResult(bool isSuccess, string message = "", float damage = 0f, float healing = 0f)
        {
            IsSuccess = isSuccess;
            Message = message ?? string.Empty;
            Damage = damage;
            Healing = healing;
            ExecutionTime = DateTime.UtcNow;
            GeneratedEffects = new List<AttributeEffect>();
            ExtraData = new Dictionary<string, object>();
        }
        
        /// <summary>
        /// 添加生成的效果
        /// </summary>
        /// <param name="effect">效果</param>
        public void AddGeneratedEffect(AttributeEffect effect)
        {
            if (effect != null)
            {
                GeneratedEffects.Add(effect);
            }
        }
        
        /// <summary>
        /// 添加多个生成的效果
        /// </summary>
        /// <param name="effects">效果列表</param>
        public void AddGeneratedEffects(IEnumerable<AttributeEffect> effects)
        {
            if (effects != null)
            {
                GeneratedEffects.AddRange(effects);
            }
        }
        
        /// <summary>
        /// 设置额外数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetExtraData(string key, object value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                ExtraData[key] = value;
            }
        }
        
        /// <summary>
        /// 获取额外数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>数据值</returns>
        public T GetExtraData<T>(string key)
        {
            if (ExtraData.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default(T);
        }
        
        /// <summary>
        /// 创建成功结果
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="damage">伤害值</param>
        /// <param name="healing">治疗值</param>
        /// <returns>成功结果</returns>
        public static AbilityActivationResult Success(string message = "技能激活成功", float damage = 0f, float healing = 0f)
        {
            return new AbilityActivationResult(true, message, damage, healing);
        }
        
        /// <summary>
        /// 创建失败结果
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <returns>失败结果</returns>
        public static AbilityActivationResult Failure(string message = "技能激活失败")
        {
            return new AbilityActivationResult(false, message);
        }
        
        /// <summary>
        /// 获取结果的字符串表示
        /// </summary>
        /// <returns>格式化的结果信息</returns>
        public override string ToString()
        {
            var result = IsSuccess ? "成功" : "失败";
            var details = new List<string>();
            
            if (Damage > 0)
                details.Add($"伤害: {Damage}");
            if (Healing > 0)
                details.Add($"治疗: {Healing}");
            if (GeneratedEffects.Count > 0)
                details.Add($"效果数量: {GeneratedEffects.Count}");
                
            var detailsStr = details.Count > 0 ? $" ({string.Join(", ", details)})" : "";
            return $"技能激活{result}: {Message}{detailsStr}";
        }
    }
}