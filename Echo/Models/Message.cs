using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Models
{
    public class Message
    {
        private readonly Client _sender;

        private readonly DateTime _timestamp;

        private readonly string _content;

        public Message(Client sender, DateTime timestamp, string content)
        {
            _sender = sender;
            _timestamp = timestamp;
            _content = content;
        }

        public Client GetSender()
        {
            return _sender;
        }

        public string GetTimestamp()
        {
            return _timestamp.ToLocalTime().ToShortDateString() + " " + _timestamp.ToLocalTime().ToShortTimeString();
        }

        public string GetVariableTimestamp()
        {
            DateTime currentDT = DateTime.Now;

            int timeCheck = DateTime.Compare(_timestamp.ToLocalTime(), currentDT);
            string hour;
            string minute;
            string day;
            string month;
            string returnTimestamp;

            if (timeCheck < 0)
            {
                if (_timestamp.ToLocalTime().Date == currentDT.Date)
                {
                    if (_timestamp.ToLocalTime().Minute < 10)
                    { minute = "0" + _timestamp.ToLocalTime().Minute.ToString(); }
                    else
                    { minute = _timestamp.ToLocalTime().Minute.ToString(); }
                    if (_timestamp.ToLocalTime().Hour < 10)
                    { hour = "0" + _timestamp.ToLocalTime().Hour.ToString(); }
                    else
                    { hour = _timestamp.ToLocalTime().Hour.ToString(); }

                    returnTimestamp = hour + ":" + minute;
                }
                else
                {
                    if (_timestamp.ToLocalTime().Day < 10)
                    { day = "0" + _timestamp.ToLocalTime().Day.ToString(); }
                    else
                    { day = _timestamp.ToLocalTime().Day.ToString(); }
                    if (_timestamp.ToLocalTime().Month < 10)
                    { month = "0" + _timestamp.ToLocalTime().Month.ToString(); }
                    else
                    { month = _timestamp.ToLocalTime().Month.ToString(); }

                    returnTimestamp = day + "/" + month;
                }
            }
            else
            {
                if (_timestamp.ToLocalTime().Minute < 10)
                { minute = "0" + _timestamp.ToLocalTime().Minute.ToString(); }
                else
                { minute = _timestamp.ToLocalTime().Minute.ToString(); }
                if (_timestamp.ToLocalTime().Hour < 10)
                { hour = "0" + _timestamp.ToLocalTime().Hour.ToString(); }
                else
                { hour = _timestamp.ToLocalTime().Hour.ToString(); }

                returnTimestamp = hour + ":" + minute;
            }

            return returnTimestamp;
        }

        public string GetContent()
        {
            return _content;
        }
    }
}
