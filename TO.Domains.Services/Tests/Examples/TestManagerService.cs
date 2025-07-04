using Godot;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.LogSystem;
using TO.Domains.Services.Abstractions.Bases;
using TO.Domains.Services.Abstractions.Tests.Examples;

namespace TO.Domains.Services.Tests.Examples;

/// <summary>
/// 场景节点管理器服务实现
/// 负责管理游戏场景中的动态节点，提供节点的创建、销毁、查询和统计功能
/// </summary>
public class TestManagerService : BasesService, ITestManagerService
{
    private readonly ILoggerRepo _logger;
    
    // 节点模板存储
    private readonly Dictionary<string, (PackedScene Scene, NodeCategory Category)> _nodeTemplates = new();
    
    // 节点实例存储
    private readonly Dictionary<string, NodeInstanceInfo> _nodeInstances = new();
    
    // 节点池限制
    private readonly Dictionary<NodeCategory, int> _poolLimits = new();
    
    // 实例计数器
    private int _instanceCounter = 0;
    
    public TestManagerService(ILoggerRepo logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        InitializeDefaultLimits();
    }
    
    /// <summary>
    /// 初始化默认的节点池限制
    /// </summary>
    private void InitializeDefaultLimits()
    {
        _poolLimits[NodeCategory.Enemy] = 100;
        _poolLimits[NodeCategory.Item] = 200;
        _poolLimits[NodeCategory.Effect] = 150;
        _poolLimits[NodeCategory.Projectile] = 300;
        _poolLimits[NodeCategory.Environment] = 50;
        _poolLimits[NodeCategory.UI] = 100;
        _poolLimits[NodeCategory.Other] = 50;
    }
    
    public void RegisterNodeTemplate(string templateId, PackedScene scene, NodeCategory category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(templateId))
                throw new ArgumentException("Template ID cannot be null or empty", nameof(templateId));
            
            if (scene == null)
                throw new ArgumentNullException(nameof(scene));
            
            if (_nodeTemplates.ContainsKey(templateId))
            {
                _logger.Warning($"Template '{templateId}' already exists, overwriting");
            }
            
            _nodeTemplates[templateId] = (scene, category);
            _logger.Info($"Registered node template '{templateId}' for category '{category}'");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to register node template '{templateId}': {ex.Message}");
            throw;
        }
    }
    
    public Node? CreateNodeInstance(string templateId, Node parent, Vector3 position = default, string? instanceId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(templateId))
                throw new ArgumentException("Template ID cannot be null or empty", nameof(templateId));
            
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            
            if (!_nodeTemplates.TryGetValue(templateId, out var template))
            {
                _logger.Error($"Template '{templateId}' not found");
                return null;
            }
            
            // 检查节点池限制
            var categoryCount = GetInstancesByCategory(template.Category).Count();
            if (categoryCount >= _poolLimits[template.Category])
            {
                _logger.Warning($"Node pool limit reached for category '{template.Category}' (limit: {_poolLimits[template.Category]})");
                return null;
            }
            
            // 生成实例ID
            var finalInstanceId = instanceId ?? $"{templateId}_{++_instanceCounter}_{DateTime.Now.Ticks}";
            
            if (_nodeInstances.ContainsKey(finalInstanceId))
            {
                _logger.Error($"Instance ID '{finalInstanceId}' already exists");
                return null;
            }
            
            // 创建节点实例
            var nodeInstance = template.Scene.Instantiate();
            if (nodeInstance == null)
            {
                _logger.Error($"Failed to instantiate scene for template '{templateId}'");
                return null;
            }
            
            // 设置位置（如果节点支持位置设置）
            if (nodeInstance is Node3D node3D)
            {
                node3D.Position = position;
            }
            else if (nodeInstance is Node2D node2D)
            {
                node2D.Position = new Vector2(position.X, position.Y);
            }
            
            // 添加到父节点
            parent.AddChild(nodeInstance);
            
            // 记录实例信息
            var instanceInfo = new NodeInstanceInfo
            {
                InstanceId = finalInstanceId,
                TemplateId = templateId,
                Category = template.Category,
                NodeReference = nodeInstance,
                CreatedTime = DateTime.Now,
                IsActive = true,
                Position = position
            };
            
            _nodeInstances[finalInstanceId] = instanceInfo;
            
            _logger.Info($"Created node instance '{finalInstanceId}' from template '{templateId}' at position {position}");
            return nodeInstance;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to create node instance from template '{templateId}': {ex.Message}");
            return null;
        }
    }
    
    public bool DestroyNodeInstance(string instanceId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(instanceId))
                throw new ArgumentException("Instance ID cannot be null or empty", nameof(instanceId));
            
            if (!_nodeInstances.TryGetValue(instanceId, out var instanceInfo))
            {
                _logger.Warning($"Instance '{instanceId}' not found");
                return false;
            }
            
            // 销毁节点
            if (instanceInfo.NodeReference != null && IsInstanceValid(instanceInfo.NodeReference))
            {
                instanceInfo.NodeReference.QueueFree();
            }
            
            // 从记录中移除
            _nodeInstances.Remove(instanceId);
            
            _logger.Info($"Destroyed node instance '{instanceId}'");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to destroy node instance '{instanceId}': {ex.Message}");
            return false;
        }
    }
    
    public int DestroyAllInstancesOfTemplate(string templateId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(templateId))
                throw new ArgumentException("Template ID cannot be null or empty", nameof(templateId));
            
            var instancesToDestroy = _nodeInstances.Values
                .Where(info => info.TemplateId == templateId)
                .ToList();
            
            var destroyedCount = 0;
            foreach (var instanceInfo in instancesToDestroy)
            {
                if (DestroyNodeInstance(instanceInfo.InstanceId))
                {
                    destroyedCount++;
                }
            }
            
            _logger.Info($"Destroyed {destroyedCount} instances of template '{templateId}'");
            return destroyedCount;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to destroy instances of template '{templateId}': {ex.Message}");
            return 0;
        }
    }
    
    public Node? GetNodeInstance(string instanceId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(instanceId))
                return null;
            
            if (_nodeInstances.TryGetValue(instanceId, out var instanceInfo))
            {
                // 检查节点是否仍然有效
                if (instanceInfo.NodeReference != null && IsInstanceValid(instanceInfo.NodeReference))
                {
                    return instanceInfo.NodeReference;
                }
                else
                {
                    // 清理无效的引用
                    _nodeInstances.Remove(instanceId);
                    _logger.Warning($"Removed invalid node reference for instance '{instanceId}'");
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get node instance '{instanceId}': {ex.Message}");
            return null;
        }
    }
    
    public IEnumerable<Node> GetInstancesOfTemplate(string templateId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(templateId))
                return Enumerable.Empty<Node>();
            
            return _nodeInstances.Values
                .Where(info => info.TemplateId == templateId && 
                              info.NodeReference != null && 
                              IsInstanceValid(info.NodeReference))
                .Select(info => info.NodeReference!)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get instances of template '{templateId}': {ex.Message}");
            return Enumerable.Empty<Node>();
        }
    }
    
    public IEnumerable<Node> GetInstancesByCategory(NodeCategory category)
    {
        try
        {
            return _nodeInstances.Values
                .Where(info => info.Category == category && 
                              info.NodeReference != null && 
                              IsInstanceValid(info.NodeReference))
                .Select(info => info.NodeReference!)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get instances by category '{category}': {ex.Message}");
            return Enumerable.Empty<Node>();
        }
    }
    
    public NodeStatistics GetStatistics()
    {
        try
        {
            // 清理无效的节点引用
            CleanupInvalidReferences();
            
            var statistics = new NodeStatistics
            {
                TotalNodes = _nodeInstances.Count,
                LastUpdateTime = DateTime.Now
            };
            
            // 按类别统计
            foreach (var category in Enum.GetValues<NodeCategory>())
            {
                statistics.NodesByCategory[category] = _nodeInstances.Values
                    .Count(info => info.Category == category);
            }
            
            // 按模板统计
            var templateGroups = _nodeInstances.Values.GroupBy(info => info.TemplateId);
            foreach (var group in templateGroups)
            {
                statistics.NodesByTemplate[group.Key] = group.Count();
            }
            
            // 活跃/非活跃节点统计
            statistics.ActiveNodes = _nodeInstances.Values.Count(info => info.IsActive);
            statistics.InactiveNodes = statistics.TotalNodes - statistics.ActiveNodes;
            
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get statistics: {ex.Message}");
            return new NodeStatistics { LastUpdateTime = DateTime.Now };
        }
    }
    
    public void ClearAllNodes()
    {
        try
        {
            var instanceIds = _nodeInstances.Keys.ToList();
            var destroyedCount = 0;
            
            foreach (var instanceId in instanceIds)
            {
                if (DestroyNodeInstance(instanceId))
                {
                    destroyedCount++;
                }
            }
            
            _logger.Info($"Cleared all nodes: {destroyedCount} instances destroyed");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to clear all nodes: {ex.Message}");
        }
    }
    
    public void SetNodePoolLimit(NodeCategory category, int maxCount)
    {
        try
        {
            if (maxCount < 0)
                throw new ArgumentException("Max count cannot be negative", nameof(maxCount));
            
            _poolLimits[category] = maxCount;
            _logger.Info($"Set node pool limit for category '{category}' to {maxCount}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to set node pool limit for category '{category}': {ex.Message}");
            throw;
        }
    }
    
    public IEnumerable<Node> CreateMultipleInstances(string templateId, Node parent, IEnumerable<Vector3> positions)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(templateId))
                throw new ArgumentException("Template ID cannot be null or empty", nameof(templateId));
            
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            
            if (positions == null)
                throw new ArgumentNullException(nameof(positions));
            
            var createdNodes = new List<Node>();
            var positionList = positions.ToList();
            
            foreach (var position in positionList)
            {
                var node = CreateNodeInstance(templateId, parent, position);
                if (node != null)
                {
                    createdNodes.Add(node);
                }
            }
            
            _logger.Info($"Created {createdNodes.Count} instances of template '{templateId}' from {positionList.Count} positions");
            return createdNodes;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to create multiple instances of template '{templateId}': {ex.Message}");
            return Enumerable.Empty<Node>();
        }
    }
    
    /// <summary>
    /// 检查节点实例是否仍然有效
    /// </summary>
    private static bool IsInstanceValid(Node? node)
    {
        return node != null && GodotObject.IsInstanceValid(node);
    }
    
    /// <summary>
    /// 清理无效的节点引用
    /// </summary>
    private void CleanupInvalidReferences()
    {
        try
        {
            var invalidInstances = _nodeInstances
                .Where(kvp => kvp.Value.NodeReference == null || !IsInstanceValid(kvp.Value.NodeReference))
                .Select(kvp => kvp.Key)
                .ToList();
            
            foreach (var instanceId in invalidInstances)
            {
                _nodeInstances.Remove(instanceId);
            }
            
            if (invalidInstances.Count > 0)
            {
                _logger.Info($"Cleaned up {invalidInstances.Count} invalid node references");
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to cleanup invalid references: {ex.Message}");
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                ClearAllNodes();
                _nodeTemplates.Clear();
                _poolLimits.Clear();
                _logger.Info("TestManagerService disposed successfully");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during TestManagerService disposal: {ex.Message}");
            }
        }
        
        base.Dispose(disposing);
    }
}