using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Echo.Managers;
using Echo.Objects;
using System.Windows.Media;
using System.Windows.Controls;

namespace Echo.Net
{
    class channelHistory
    {
        public static void Handle(Dictionary<string, string> message)
        {
            List<List<string>> channelHistory = JsonConvert.DeserializeObject<List<List<string>>>(message["data"]);

            MainWindow.main.Dispatcher.Invoke(() => {
                foreach (List<string> m in channelHistory)
                {
                    Message formattedMessage = new Message(m[0], m[3], m[2], m[4]);
                    MainWindow.main.listBoxMessages.Items.Add(formattedMessage);
                }

                if (VisualTreeHelper.GetChildrenCount(MainWindow.main.listBoxMessages) > 0)
                {
                    Border border = (Border)VisualTreeHelper.GetChild(MainWindow.main.listBoxMessages, 0);
                    ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                    scrollViewer.ScrollToBottom();
                }
            });
        }
    }
}