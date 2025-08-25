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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autofac;
using Soundboard;
using Soundboard.ViewModels;

namespace Soundboard.Views
{
    /// <summary>
    /// Interaction logic for CrudToolbar.xaml
    /// </summary>
    public partial class CrudToolbar : UserControl
    {
        public CrudToolbar()
        {
            InitializeComponent();
            
            if(DataContext == null)
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
                    DataContext = app.Container.Resolve<CrudToolbarViewModel>();
                }
            }
            catch
            {
                throw new Exception("Failed to resolve CrudToolbarViewModel");
            }
        }
    }
}

// Set DataContext if not already set (design time vs runtime)
//if (DataContext == null)
//{
//    SetDataContextFromContainer();
//}
//    }

//private void SetDataContextFromContainer()
//{
//    try
//    {
//        var app = Application.Current as App;
//        if (app?.Container != null)
//        {
//            DataContext = app.Container.Resolve<ButtonGridViewModel>();
//        }
//    }
//    catch
//    {
//        throw new Exception("Failed to resolve ButtonGridViewModel");
//    }
//}
