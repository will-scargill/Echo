using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Echo.Managers;

namespace Echo.Views
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        public ChatView()
        {
            InitializeComponent();

            VisualManager.setMessageBox(listBoxMessages);
        }

        private void scrollChanged(object sender, ScrollChangedEventArgs e)
        {          
            Border border = (Border)VisualTreeHelper.GetChild(listBoxMessages, 0);
            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);       

            //Debug.WriteLine(scrollViewer.VerticalOffset);

            if (scrollViewer.VerticalOffset == 0 &&
                scrollViewer.IsVisible &&
                NetworkManager.getServer().currentChannelMessageList.Count >= 50 &&
                NetworkManager.blockHistory == false)
            {
                NetworkManager.blockHistory = true;
                NetworkManager.getServer().SendMessageToServer("historyRequest", "");
                Mouse.OverrideCursor = Cursors.Wait;
            }        
        }
    }
}
