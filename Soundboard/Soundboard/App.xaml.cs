using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;
using Autofac;
using Soundboard.Services;
using Soundboard.ViewModels;
using IContainer = Autofac.IContainer;

namespace Soundboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IContainer _container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigureContainer();

            var mainWindow = _container.Resolve<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            // Register services
            builder.RegisterType<AudioService>().As<IAudioService>().SingleInstance();

            // Register ViewModels
            builder.RegisterType<MainViewModel>().AsSelf();

            // Register Views
            builder.Register(c => new MainWindow
            {
                DataContext = c.Resolve<MainViewModel>()
            }).AsSelf();

            _container = builder.Build();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _container?.Dispose();
            base.OnExit(e);
        }
    }

}
