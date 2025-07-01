using TO.Domains.Models.Repositories.Abstractions.Core.UISystem;
using TO.Domains.Services.Abstractions.Bases;
using TO.Domains.Services.Abstractions.Core.UISystem;
using TO.Nodes.Abstractions.Nodes.UI.Bases;

namespace Domains.Core.UISystem;

public class UILayerService(IUIManagerRepo uiManagerRepo) : BasesService,IUILayerService
{
    
    public void HandleShowScreenLayerRelation(IUIScreen newScreen, IUIScreen? currentScreen)
    {
        if (currentScreen == null) return;
        
        if (currentScreen.LayerType == newScreen.LayerType  && !newScreen.IsTransparent)
        {
            currentScreen.Hide();
        }
    }
    

}
