using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soundboard.Domain.DataAccess.Implementations;
using Soundboard.Views;

namespace Soundboard.Services;

public interface IPrompter
{
    string PromptForText(string title, string message, string defaultText = "");
    bool PromptForConfirmation(string title, string message);
    SoundButtonGridLayout PromptForGridSelection(string title, List<SoundButtonGridLayout> availableGrids, ISoundboardRepository repository);
}

public class Prompter : IPrompter
{
    public string PromptForText(string title, string message, string defaultText = "")
    {
        var promptWindow = new PromptWindow(title, message, defaultText);
        var result = promptWindow.ShowDialog();

        return result == true ? promptWindow.ResponseText : null;
    }

    public bool PromptForConfirmation(string title, string message)
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Question);
        return result == MessageBoxResult.OK;
    }

    public SoundButtonGridLayout PromptForGridSelection(string title, List<SoundButtonGridLayout> availableGrids, ISoundboardRepository repository)
    {
        var gridSelectionWindow = new GridSelectionWindow(title, availableGrids, repository); 
        var result = gridSelectionWindow.ShowDialog();

        return result == true ? gridSelectionWindow.SelectedGrid : null;
    }
}
