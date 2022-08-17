using Echo.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Echo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.sbvText.Text = "Echo Version " + ConfigManager.GetSetting("version");

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NetworkManager.getServer()?.Disconnect();
            System.Windows.Application.Current.Shutdown();
        }
    }
}
