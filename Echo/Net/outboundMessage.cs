using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

using Echo.Objects;

namespace Echo.Net
{
    class outboundMessage
    {
        public static void Handle(Dictionary<string, string> message)
        {
            MainWindow.main.Dispatcher.Invoke(() => {

                string messageContent = message["data"];

                List<string> metadata = JsonConvert.DeserializeObject<List<string>>(message["metadata"]);

                //List<string> metadata = JsonConvert.DeserializeObject<List<string>>(metadataJson);

                Message newMessage = new Message(metadata[0], messageContent, metadata[2], metadata[1]);
                


                MainWindow.main.listBoxMessages.Items.Add(newMessage);

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
