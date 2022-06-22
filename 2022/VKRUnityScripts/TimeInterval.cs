using System;

public struct TimeInterval
{
    public readonly byte StartHour;
    public readonly byte StartMinute;

    public readonly byte EndHour;
    public readonly byte EndMinute;

    public TimeInterval(string start, string end)
    {
        var startArray = start.Split(':');
        var endArray = end.Split(':');

        bool paramsAreValid = true;

        paramsAreValid &= byte.TryParse(startArray[0], out StartHour);
        paramsAreValid &= byte.TryParse(startArray[1], out StartMinute);
        paramsAreValid &= byte.TryParse(endArray[0], out EndHour);
        paramsAreValid &= byte.TryParse(endArray[1], out EndMinute);

        paramsAreValid &= StartHour <= 23 && EndHour <= 23
            && StartMinute <= 59 && EndMinute <= 59;

        if (!paramsAreValid)
            throw new ArgumentException();
    }

    public TimeInterval(byte startHour, byte endHour)
    {
        if (startHour < 0 || startHour > 23
            || endHour < 0 || endHour > 23)
            throw new ArgumentException();
        StartHour = startHour;
        StartMinute = 0;
        EndHour = endHour;
        EndMinute = 0;
    }

    public override string ToString()
    {
        return $"{StartHour}:{StartMinute}-{EndHour}:{EndMinute}";
    }
}
