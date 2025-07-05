using Godot;
using TO.Commons.Enums;
using TO.Domains.Services.Abstractions.Bases;

namespace TO.Domains.Services.Abstractions.Tests.Examples;

/// <summary>
/// 场景节点管理器服务接口
/// 负责管理游戏场景中的动态节点，如敌人、道具、特效等
/// </summary>
public interface ITestManagerService 
{
    /// <summary>
    /// 注册节点模板
    /// </summary>
    /// <param name="templateId">模板ID</param>
    /// <param name="scene">场景资源</param>
    /// <param name="category">节点类别</param>
    void RegisterNodeTemplate(string templateId, PackedScene scene, NodeCategory category);
    
    /// <summary>
    /// 创建节点实例
    /// </summary>
    /// <param name="templateId">模板ID</param>
    /// <param name="parent">父节点</param>
    /// <param name="position">初始位置</param>
    /// <param name="instanceId">实例ID（可选）</param>
    /// <returns>创建的节点实例</returns>
    Node? CreateNodeInstance(string templateId, Node parent, Vector3 position = default, string? instanceId = null);
    
    /// <summary>
    /// 销毁节点实例
    /// </summary>
    /// <param name="instanceId">实例ID</param>
    /// <returns>是否成功销毁</returns>
    bool DestroyNodeInstance(string instanceId);
    
    /// <summary>
    /// 销毁指定模板的所有节点
    /// </summary>
    /// <param name="templateId">模板ID</param>
    /// <returns>销毁的节点数量</returns>
    int DestroyAllInstancesOfTemplate(string templateId);
    
    /// <summary>
    /// 获取节点实例
    /// </summary>
    /// <param name="instanceId">实例ID</param>
    /// <returns>节点实例</returns>
    Node? GetNodeInstance(string instanceId);
    
    /// <summary>
    /// 获取指定模板的所有节点实例
    /// </summary>
    /// <param name="templateId">模板ID</param>
    /// <returns>节点实例列表</returns>
    IEnumerable<Node> GetInstancesOfTemplate(string templateId);
    
    /// <summary>
    /// 获取指定类别的所有节点实例
    /// </summary>
    /// <param name="category">节点类别</param>
    /// <returns>节点实例列表</returns>
    IEnumerable<Node> GetInstancesByCategory(NodeCategory category);
    
    /// <summary>
    /// 获取节点统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    NodeStatistics GetStatistics();
    
    /// <summary>
    /// 清理所有管理的节点
    /// </summary>
    void ClearAllNodes();
    
    /// <summary>
    /// 设置节点池大小限制
    /// </summary>
    /// <param name="category">节点类别</param>
    /// <param name="maxCount">最大数量</param>
    void SetNodePoolLimit(NodeCategory category, int maxCount);
    
    /// <summary>
    /// 批量创建节点
    /// </summary>
    /// <param name="templateId">模板ID</param>
    /// <param name="parent">父节点</param>
    /// <param name="positions">位置列表</param>
    /// <returns>创建的节点实例列表</returns>
    IEnumerable<Node> CreateMultipleInstances(string templateId, Node parent, IEnumerable<Vector3> positions);
}

/// <summary>
/// 节点类别枚举
/// </summary>
public enum NodeCategory
{
    Enemy,      // 敌人
    Item,       // 道具
    Effect,     // 特效
    Projectile, // 投射物
    Environment,// 环境物体
    UI,         // UI元素
    Other       // 其他
}

/// <summary>
/// 节点统计信息
/// </summary>
public class NodeStatistics
{
    public int TotalNodes { get; set; }
    public Dictionary<NodeCategory, int> NodesByCategory { get; set; } = new();
    public Dictionary<string, int> NodesByTemplate { get; set; } = new();
    public int ActiveNodes { get; set; }
    public int InactiveNodes { get; set; }
    public DateTime LastUpdateTime { get; set; }
}

/// <summary>
/// 节点实例信息
/// </summary>
public class NodeInstanceInfo
{
    public string InstanceId { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public NodeCategory Category { get; set; }
    public Node? NodeReference { get; set; }
    public DateTime CreatedTime { get; set; }
    public bool IsActive { get; set; }
    public Vector3 Position { get; set; }
}