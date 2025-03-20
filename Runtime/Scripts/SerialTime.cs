using System;
using Bakery.Saves;
namespace Bakery.TimeOfDay
{
    public class SerialTime : SerialData
    {
        public string Time;

        public SerialTime()
        {
            Time = null;
        }

        public SerialTime(DateTime time)
        {
            Time = time.ToString();
        }

        public override string Key() => "TimeOfDay";

    }
}
