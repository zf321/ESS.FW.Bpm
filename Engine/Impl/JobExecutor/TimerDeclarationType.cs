using System;
using System.Collections.Generic;
using System.ComponentModel;
using ESS.FW.Bpm.Engine.Impl.Calendar;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    //public sealed class TimerDeclarationType
    //{
    //    public enum InnerEnum
    //    {
    //        Date,
    //        Duration,
    //        Cycle
    //    }

    //    public static readonly TimerDeclarationType Date = new TimerDeclarationType("DATE", InnerEnum.Date,
    //        DueDateBusinessCalendar.Name);

    //    public static readonly TimerDeclarationType Duration = new TimerDeclarationType("DURATION", InnerEnum.Duration,
    //        DurationBusinessCalendar.Name);

    //    public static readonly TimerDeclarationType Cycle = new TimerDeclarationType("CYCLE", InnerEnum.Cycle,
    //        CycleBusinessCalendar.Name);

    //    private static readonly IList<TimerDeclarationType> ValueList = new List<TimerDeclarationType>();
    //    private static int _nextOrdinal;

    //    public readonly string CalendarName;
    //    private readonly InnerEnum _innerEnumValue;

    //    private readonly string _nameValue;
    //    private readonly int _ordinalValue;

    //    static TimerDeclarationType()
    //    {
    //        //valueList.Add(DATE);
    //        ValueList.Add(Duration);
    //        ValueList.Add(Cycle);
    //    }

    //    internal TimerDeclarationType(string name, InnerEnum innerEnum, string caledarName)
    //    {
    //        CalendarName = caledarName;

    //        _nameValue = name;
    //        _ordinalValue = _nextOrdinal++;
    //        _innerEnumValue = innerEnum;
    //    }

    //    public static IList<TimerDeclarationType> Values()
    //    {
    //        return ValueList;
    //    }

    //    public InnerEnum InnerEnumValue()
    //    {
    //        return _innerEnumValue;
    //    }

    //    public int Ordinal()
    //    {
    //        return _ordinalValue;
    //    }

    //    public override string ToString()
    //    {
    //        return _nameValue;
    //    }

    //    public static TimerDeclarationType ValueOf(string name)
    //    {
    //        foreach (var enumInstance in Values())
    //            if (enumInstance._nameValue == name)
    //                return enumInstance;
    //        throw new ArgumentException(name);
    //    }
    //}

    public enum TimerDeclarationType
    {
        [Description(DueDateBusinessCalendar.Name)]
        Date,

        [Description(DurationBusinessCalendar.Name)]
        Duration,

        [Description(CycleBusinessCalendar.Name)]
        Cycle
    }
}