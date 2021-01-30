using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Echo.Objects
{
    class Message
    {
        public string Username { get; set; }

        public string Content { get; set; }

        private string _datetime;
        private string hour;
        private string minute;
        private string day;
        private string month;
        public string Datetime
        {
            get { return this._datetime; }
            set
            {
                DateTime valDT = Convert.ToDateTime(value.ToString());
                DateTime currentDT = DateTime.Now;

                int timeCheck = DateTime.Compare(valDT, currentDT);

                if (timeCheck < 0)
                {
                    if (valDT.Date == currentDT.Date)
                    {
                        if (valDT.Minute < 10)
                        { minute = "0" + valDT.Minute.ToString(); }
                        else
                        { minute = valDT.Minute.ToString(); }
                        if (valDT.Hour < 10)
                        { hour = "0" + valDT.Hour.ToString(); }
                        else
                        { hour = valDT.Hour.ToString(); }

                        this._datetime = hour + ":" + minute;
                    }
                    else
                    {
                        if (valDT.Day < 10)
                        { day = "0" + valDT.Day.ToString(); }
                        else
                        { day = valDT.Day.ToString(); }
                        if (valDT.Month < 10)
                        { month = "0" + valDT.Month.ToString(); }
                        else
                        { month = valDT.Month.ToString(); }

                        this._datetime = day + "/" + month;
                    }
                }
                else if (timeCheck == 0)
                {
                    if (valDT.Minute < 10)
                    { minute = "0" + valDT.Minute.ToString(); }
                    else
                    { minute = valDT.Minute.ToString(); }
                    if (valDT.Hour < 10)
                    { hour = "0" + valDT.Hour.ToString(); }
                    else
                    { hour = valDT.Hour.ToString(); }

                    this._datetime = hour + ":" + minute;
                }
            }
        }

        public string Colour { get; set; }

        public Message(string _username, string _content, string _datetime, string _colour)
        {
            Username = _username;
            Content = _content;
            Datetime = _datetime;
            Colour = _colour;
        }
    }
}
