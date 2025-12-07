using HMV_Player.Data;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.Services;

namespace HMV_Player.MVVM.ViewModels;

public partial class HomePageViewModel : PageViewModel {
    public override ApplicationPageName PageName => ApplicationPageName.Home;
    
    public HomePageViewModel(NogasmAnalyzerService nog) {
        
    }
}