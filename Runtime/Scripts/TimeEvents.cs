using System;

public static class TimeEvents
{
    public static Action<DateTime> OnTimeChanged = delegate { };
    public static Action OnDayChanged = delegate { };
    public static Action<int> OnHourTicked = (numHours) => { };
    internal static Action<int> OnTimeFactorChanged = delegate { };
}
