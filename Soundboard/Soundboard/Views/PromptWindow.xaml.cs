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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Soundboard.Views
{
    /// <summary>
    /// Interaction logic for PromptWindow.xaml
    /// </summary>
    public partial class PromptWindow : Window
    {
        new public string Title { get; }
        public string Message { get; }
        public string ResponseText { get; set; }

        public PromptWindow(string title, string message, string defaultText = "")
        {
            InitializeComponent();

            Title = title;
            Message = message;
            ResponseText = defaultText;

            DataContext = this;

            Loaded += (s, e) => ResponseTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ResponseText))
            {
                DialogResult = false;
            }
            else
            {
                DialogResult = true;
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
