using System;
using HMV_Player.Data;
using HMV_Player.MVVM.ViewModels;
using HMV_Player.MVVM.ViewModels.Base;

namespace HMV_Player.Factories;

public class PageFactory(Func<ApplicationPageName, PageViewModel> factory) {

    public PageViewModel GetPageViewModel(ApplicationPageName pageName) => factory.Invoke(pageName);
}