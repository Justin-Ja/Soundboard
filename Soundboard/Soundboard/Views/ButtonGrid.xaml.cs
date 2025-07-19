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
using Autofac;
using Soundboard.ViewModels;

namespace Soundboard.Views;
    /// <summary>
    /// Interaction logic for ButtonGrid.xaml
    /// </summary>

public partial class ButtonGrid : UserControl
{
    public ButtonGrid()
    {
        InitializeComponent();

        // Set DataContext if not already set (design time vs runtime)
        if (DataContext == null)
        {
            SetDataContextFromContainer();
        }
    }

    private void SetDataContextFromContainer()
    {
        try
        {
            var app = Application.Current as App;
            if (app?.Container != null)
            {
                DataContext = app.Container.Resolve<ButtonGridViewModel>();
            }
        }
        catch
        {
            throw new Exception("Failed to resolve ButtonGridViewModel");
        }
    }
}
