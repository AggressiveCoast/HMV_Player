using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace HMV_Player.Services.DialogueWindow;

public class DialogueService(Func<TopLevel> _topLevel) : IDialogueService {
    
    public async Task<string?> OpenFolderSelectorAsync(string title = "Select Folder") {
        var folders = await _topLevel.Invoke().StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions() {
            Title = title,
            AllowMultiple = false
        });

        return folders.Count > 0 ? folders[0].Path.LocalPath : null;
    }
}