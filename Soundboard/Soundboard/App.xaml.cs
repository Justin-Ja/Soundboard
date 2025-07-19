using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;
using Autofac;
using Soundboard.Services;
using Soundboard.ViewModels;
using Soundboard.Views;
using IContainer = Autofac.IContainer;

namespace Soundboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IContainer _container;

        //Dont like this but this works for app running with button grid. Should look into cleaning up this DI thing i think its gonna spiral soon :(
        public IContainer Container => _container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigureContainer();

            var mainWindow = _container.Resolve<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureContainer()
        {
            //TODO: We can create the container/builder here, but should pass it into Modules relating to diff projects.
            //ie we have a Soundboard Module, a Service Module, a settings module (If we add a settings section).
            //Also would help with keeping this short i dont like a xaml.cs to be long-winded
            var builder = new ContainerBuilder();

            builder.RegisterType<AudioService>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<MainViewModel>().AsSelf();

            builder.RegisterType<ButtonGridViewModel>().AsSelf().SingleInstance();

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
