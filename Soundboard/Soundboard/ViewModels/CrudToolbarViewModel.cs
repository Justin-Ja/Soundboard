using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Soundboard.Commands;
using Soundboard.Common;
using Soundboard.Common.Events;
using Soundboard.Domain.DataAccess.Implementations;
using Soundboard.Services;

namespace Soundboard.ViewModels;

public class CrudToolbarViewModel : BaseViewModel, ICrudToolbarViewModel
{
    private readonly ISoundboardRepository _repository;
    private readonly IPrompter _prompter;
    private SoundButtonGridLayout _currentGrid;
    private string _currentGridName;

    public string CurrentGridName
    {
        get => _currentGridName;
        private set => SetProperty(ref _currentGridName, value);
    }

    public SoundButtonGridLayout CurrentGrid
    {
        get => _currentGrid;
        private set
        {
            if (SetProperty(ref _currentGrid, value))
            {
                CurrentGridName = value?.Name ?? "No Grid Selected";
            }
        }
    }

    public ICommand NewGridCommand { get; }
    public ICommand LoadGridCommand { get; }
    public ICommand SaveGridCommand { get; }
    public ICommand SaveAsGridCommand { get; }

    //Event to communicate with ButtonGrid
    public event EventHandler<GridChangedEventArgs> GridChanged;

    public CrudToolbarViewModel(ISoundboardRepository repository, IPrompter prompter)
    {
        _repository = repository;
        _prompter = prompter;

        NewGridCommand = new RelayCommand(async () => await NewGridAsync(), () => true);
        LoadGridCommand = new RelayCommand(async () => await LoadGridAsync(), () => true);
        SaveGridCommand = new RelayCommand(async () => await SaveGridAsync(), () => CanSaveGrid());
        SaveAsGridCommand = new RelayCommand(async () => await SaveAsGridAsync(), () => CanSaveAsGrid());

        CurrentGridName = "No Grid Selected";
    }

    private bool CanSaveGrid()
    {
        return CurrentGrid != null;
    }

    private bool CanSaveAsGrid()
    {
        return CurrentGrid != null;
    }

    private async Task NewGridAsync()
    {
        try
        {
            var gridName = _prompter.PromptForText("New Grid", "Enter a name for the new button grid:", "");

            if (string.IsNullOrWhiteSpace(gridName))
                return;

            var newGrid = new SoundButtonGridLayout
            {
                Guid = Guid.NewGuid(),
                Name = gridName.Trim(),
                SoundButtons = new List<SoundButton>()
            };

            var savedGrid = await _repository.AddButtonGridWithSoundButtonsAsync(newGrid);
            CurrentGrid = savedGrid;

            //Notify ButtonGrid to load this new grid
            OnGridChanged(savedGrid, "New");
        }
        catch (Exception ex)
        {
            _prompter.PromptForConfirmation("Error", $"Failed to create new grid: {ex.Message}");
        }
    }

    private async Task LoadGridAsync()
    {
        try
        {
            var availableGrids = await _repository.GetAllButtonGridsAsync();

            if (!availableGrids.Any())
            {
                _prompter.PromptForConfirmation("No Grids", "No button grids found. Create a new grid first.");
                return;
            }

            var selectedGrid = _prompter.PromptForGridSelection("Load Button Grid", availableGrids);

            if (selectedGrid != null)
            {
                CurrentGrid = selectedGrid;

                //Notify ButtonGrid to load this grid
                OnGridChanged(selectedGrid, "Load");
            }
        }
        catch (Exception ex)
        {
            _prompter.PromptForConfirmation("Error", $"Failed to load grid: {ex.Message}");
        }
    }

    private async Task SaveGridAsync()
    {
        if (CurrentGrid == null) return;

        try
        {
            var gridData = RequestCurrentGridData();
            if (gridData != null)
            {
                CurrentGrid.SoundButtons = gridData;
                var updatedGrid = await _repository.UpdateButtonGridAsync(CurrentGrid);
            }

            _prompter.PromptForConfirmation("Success", $"Grid '{CurrentGrid.Name}' saved successfully!");
        }
        catch (Exception ex)
        {
            _prompter.PromptForConfirmation("Error", $"Failed to save grid: {ex.Message}");
        }
    }

    private async Task SaveAsGridAsync()
    {
        if (CurrentGrid == null) return;

        try
        {
            var newGridName = _prompter.PromptForText("Save As", "Enter a name for the copied grid:", $"{CurrentGrid.Name} - Copy");

            if (string.IsNullOrWhiteSpace(newGridName))
                return;

            //Request current button data from ButtonGrid
            var gridData = RequestCurrentGridData();

            var newGrid = new SoundButtonGridLayout
            {
                Guid = Guid.NewGuid(),
                Name = newGridName.Trim(),
                SoundButtons = gridData ?? new List<SoundButton>()
            };

            var savedGrid = await _repository.AddButtonGridWithSoundButtonsAsync(newGrid);
            CurrentGrid = savedGrid;

            _prompter.PromptForConfirmation("Success", $"Grid saved as '{newGrid.Name}' successfully!");
        }
        catch (Exception ex)
        {
            _prompter.PromptForConfirmation("Error", $"Failed to save grid copy: {ex.Message}");
        }
    }

    private List<SoundButton> RequestCurrentGridData()
    {
        //This method would need to communicate with the ButtonGrid to get current state - do so with events
        //This will raise an event that ButtonGrid subscribes to to know how/when to update
        var args = new GridDataRequestEventArgs();
        GridDataRequested?.Invoke(this, args);
        return args.GridData;

    }

    public event EventHandler<GridDataRequestEventArgs> GridDataRequested;

    protected virtual void OnGridChanged(SoundButtonGridLayout grid, string operation)
    {
        GridChanged?.Invoke(this, new GridChangedEventArgs(grid, operation));
    }

    public class GridDataRequestEventArgs : EventArgs
    {
        public List<SoundButton> GridData { get; set; }
    }
}
