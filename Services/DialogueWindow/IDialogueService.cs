using System.Threading.Tasks;

namespace HMV_Player.Services.DialogueWindow;

public interface IDialogueService {

    public Task<string?> OpenFolderSelectorAsync(string title = "Select Folder");
}