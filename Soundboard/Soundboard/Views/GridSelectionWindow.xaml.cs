using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Soundboard.Domain.DataAccess.Implementations;

namespace Soundboard.Views
{
    /// <summary>
    /// Interaction logic for GridSelectionWindow.xaml
    /// </summary>
    public partial class GridSelectionWindow : Window
    {
        public string Title { get; }
        public List<SoundButtonGridLayout> AvailableGrids { get; }
        public SoundButtonGridLayout SelectedGrid { get; set; }

        public GridSelectionWindow(string title, List<SoundButtonGridLayout> availableGrids)
        {
            InitializeComponent();

            Title = title;
            AvailableGrids = availableGrids;

            DataContext = this;

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
