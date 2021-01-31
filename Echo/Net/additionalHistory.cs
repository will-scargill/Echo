using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

using Echo.Objects;
using System.Windows.Controls;


namespace Echo.Net
{
    class additionalHistory
    {
        public static void Handle(Dictionary<string, string> message)
        {
            List<List<string>> channelHistory = JsonConvert.DeserializeObject<List<List<string>>>(message["data"]);
            int newMessages = channelHistory.Count;

            MainWindow.main.Dispatcher.Invoke(() => {
                object posRetain = MainWindow.main.listBoxMessages.Items.GetItemAt(1);

                foreach (List<string> m in channelHistory)
                {
                    Message formattedMessage = new Message(m[0], m[3], m[2], m[4]);
                    MainWindow.main.listBoxMessages.Items.Insert(0,formattedMessage);
                }

                Border border = (Border)VisualTreeHelper.GetChild(MainWindow.main.listBoxMessages, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                MainWindow.main.listBoxMessages.ScrollIntoView(posRetain);
                int visibleItems = Convert.ToInt32(scrollViewer.ViewportHeight);
                MainWindow.main.listBoxMessages.SelectedItem = posRetain;
                int currentIndex = MainWindow.main.listBoxMessages.SelectedIndex;
                object scrollDownTo = MainWindow.main.listBoxMessages.Items.GetItemAt(currentIndex + visibleItems - 2);
                MainWindow.main.listBoxMessages.ScrollIntoView(scrollDownTo);

        });
        }
    }
}
