using Echo.Models;
using Echo.Stores;
using Echo.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Echo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly EchoClient _echo;
        private readonly NavigationStore _navigationStore;
        public App()
        {
            _navigationStore = new NavigationStore();
            _echo = new EchoClient(_navigationStore);     
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _navigationStore.CurrentViewModel = new ConnectionViewModel(_echo, _navigationStore);

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_echo, _navigationStore)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
