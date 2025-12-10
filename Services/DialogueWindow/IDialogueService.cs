using System.Threading.Tasks;
using HMV_Player.MVVM.Models;

namespace HMV_Player.Services.DialogueWindow;

public interface IDialogueService {

    public Task<string?> OpenFolderSelectorAsync(string title = "Select Folder");
    public Task<string?> OpenFileSelectorAsync(string title = "Select File");
}