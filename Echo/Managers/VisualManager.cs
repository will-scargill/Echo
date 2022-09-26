using Echo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Echo.Managers
{
    class VisualManager
    {
        private static ListBox messageBox;
        public static SolidColorBrush ConnStatusToBrush(ConnectionStatus connStatus)
        {
            switch (connStatus)
            {
                case ConnectionStatus.Idle:
                    return new SolidColorBrush(Colors.LightGray);
                case ConnectionStatus.Connecting:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34b4eb"));
                case ConnectionStatus.Connected:
                    return new SolidColorBrush(Colors.Green);
                case ConnectionStatus.Disconnected:
                    return new SolidColorBrush(Colors.LightGray);
                case ConnectionStatus.Reconnecting:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34b4eb"));
                case ConnectionStatus.Terminated:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff7f17"));
                case ConnectionStatus.Error:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff2929"));
                default:
                    return new SolidColorBrush(Colors.Purple);
            }
        }

        public static string ConnStatusToText(ConnectionStatus connStatus, string context="")
        {
            switch (connStatus)
            {
                case ConnectionStatus.Idle:
                    return "Idle";
                case ConnectionStatus.Connecting:
                    return "Connecting";
                case ConnectionStatus.Connected:
                    return "Connected";
                case ConnectionStatus.Disconnected:
                    return "Disconnected";
                case ConnectionStatus.Reconnecting:
                    return "Connection Lost - Attempting to reconnect";
                case ConnectionStatus.Terminated:
                    string termStatusText = "Terminated - " + context;
                    return termStatusText;
                case ConnectionStatus.Error:
                    string errStatusText = "Error - " + context;
                    return errStatusText;
                default:
                    return "Invalid connection status";
            }
        }

        public static DateTime UnixToDateTime(string timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(Convert.ToDouble(timestamp)).ToLocalTime();
            return dateTime;
        }
        
        public static void setMessageBox(ListBox lb)
        {
            messageBox = lb;

            try
            {
                Border border = (Border)VisualTreeHelper.GetChild(lb, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
            catch(System.ArgumentOutOfRangeException)
            {
                Debug.WriteLine("threw an error");   
            }         
        }

        public static ListBox getMessageBox()
        {
            return messageBox;
        }

        public class Themes
        {
            public static bool LoadTheme(string themeKey)
            {
                throw new NotImplementedException();
            }

            public static bool InstallTheme(Dictionary<string, string> themeData)
            {
                throw new NotImplementedException();
            }
        }
    }
}
