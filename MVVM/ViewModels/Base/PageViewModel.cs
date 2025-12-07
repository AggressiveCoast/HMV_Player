using HMV_Player.Data;

namespace HMV_Player.MVVM.ViewModels.Base;

public abstract class PageViewModel : ViewModelBase {
    
    public abstract ApplicationPageName PageName { get; }
}