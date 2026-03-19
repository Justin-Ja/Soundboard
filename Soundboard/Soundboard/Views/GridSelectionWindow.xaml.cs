using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Soundboard.Domain.DataAccess.Implementations;
using Soundboard.ViewModels;

namespace Soundboard.Views
{
    /// <summary>
    /// Interaction logic for GridSelectionWindow.xaml
    /// </summary>
    public partial class GridSelectionWindow : Window
    {
        private readonly GridSelectionViewModel _viewModel;

        public SoundButtonGridLayout SelectedGrid => _viewModel.SelectedGrid;

        public GridSelectionWindow(string title, List<SoundButtonGridLayout> availableGrids, ISoundboardRepository repository)
        {
            InitializeComponent();
            _viewModel = new GridSelectionViewModel(title, availableGrids, repository);
            DataContext = _viewModel;
            Loaded += (s, e) => GridListBox.Focus();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
