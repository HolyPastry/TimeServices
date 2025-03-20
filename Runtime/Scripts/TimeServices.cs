using System;
using UnityEngine;

public static partial class TimeServices
{
    public static Func<WaitUntil> WaitUntilReady = () => new(() => true);

    public static Action<DateTime> SetTimeOfDay = delegate { };
    public static Func<DateTime> GetTimeOfDay = () => default;

    public static Action<int> AddHours = delegate { };
    public static Func<int, CustomYieldInstruction> WaitForHours = (hours) => null;
    public static Action<int> SetHour;
    public static Action<int> SetTimeFactor = delegate { };

    public static Action PauseTimeOfDay = delegate { };
    public static Action ResumeTimeOfDay = delegate { };


    public static Func<int, int, int, int, bool> IsTimeBetween = delegate { return false; };

}
