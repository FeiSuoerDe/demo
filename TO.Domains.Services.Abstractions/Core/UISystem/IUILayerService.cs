using TO.Nodes.Abstractions.Nodes.UI.Bases;

namespace TO.Domains.Services.Abstractions.Core.UISystem;

/// <summary>
/// UI层级管理服务接口，负责处理UI屏幕的层级关系和显示规则
/// </summary>
public interface IUILayerService
{
    void HandleShowScreenLayerRelation(IUIScreen newScreen, IUIScreen? currentScreen);
}
