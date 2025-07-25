using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Soundboard.ViewModels;

namespace Soundboard.Views
{
    /// <summary>
    /// Interaction logic for KeyBindingDialog.xaml
    /// </summary>
    public partial class KeyBindingDialog : Window
    {
        //TODO: CONTAINRE AUTOFAC THIS!!!!! NO NEW!!!!
        private KeyBindingDialogViewModel _viewModel; 

        public Key? SelectedKey => _viewModel.SelectedKey;
        public ModifierKeys SelectedModifiers => _viewModel.SelectedModifiers;
        public bool IsCleared => _viewModel.IsCleared;

        public KeyBindingDialog(string soundName, Key? currentKey = null, ModifierKeys currentModifiers = ModifierKeys.None)
        {
            InitializeComponent();

            _viewModel = new KeyBindingDialogViewModel(soundName, currentKey, currentModifiers);
            _viewModel.RequestClose += OnRequestClose;
            DataContext = _viewModel;

            Loaded += (s, e) => Focus();
        }

        private void OnRequestClose(bool dialogResult)
        {
            DialogResult = dialogResult;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            _viewModel.OnKeyPressed(e.Key, Keyboard.Modifiers);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_viewModel != null)
                _viewModel.RequestClose -= OnRequestClose;
            base.OnClosed(e);
        }
    }
}
