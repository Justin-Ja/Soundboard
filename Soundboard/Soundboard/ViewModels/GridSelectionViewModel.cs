using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Soundboard.Commands;
using Soundboard.Domain.DataAccess.Implementations;

namespace Soundboard.ViewModels
{
    public class GridSelectionViewModel
    {
        private readonly ISoundboardRepository _repository;
        private SoundButtonGridLayout _selectedGrid;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public string Title { get; }
        public ObservableCollection<SoundButtonGridLayout> AvailableGrids { get; }

        public SoundButtonGridLayout SelectedGrid
        {
            get => _selectedGrid;
            set
            {
                _selectedGrid = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedGrid));
            }
        }

        public bool HasSelectedGrid => SelectedGrid != null;

        public ICommand DeleteGridCommand { get; }

        public GridSelectionViewModel(string title, List<SoundButtonGridLayout> availableGrids, ISoundboardRepository repository)
        {
            Title = title;
            AvailableGrids = new ObservableCollection<SoundButtonGridLayout>(availableGrids);
            _repository = repository;
            DeleteGridCommand = new RelayCommand(async () => await DeleteSelectedGridAsync(), () => HasSelectedGrid);
        }

        private async Task DeleteSelectedGridAsync()
        {
            if (SelectedGrid == null) return;

            await _repository.DeleteButtonGridAsync(SelectedGrid.Guid);
            AvailableGrids.Remove(SelectedGrid);
            SelectedGrid = null;
        }
    }
}
