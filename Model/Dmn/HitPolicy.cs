using System.Collections.Generic;
using System.ComponentModel;

namespace ESS.FW.Bpm.Model.Dmn
{

    //public sealed class HitPolicy
    //{

    //    public static readonly HitPolicy Unique = new HitPolicy("UNIQUE", InnerEnum.Unique);
    //    public static readonly HitPolicy First = new HitPolicy("FIRST", InnerEnum.First);
    //    public static readonly HitPolicy Priority = new HitPolicy("PRIORITY", InnerEnum.Priority);
    //    public static readonly HitPolicy Any = new HitPolicy("ANY", InnerEnum.Any);
    //    public static readonly HitPolicy Collect = new HitPolicy("COLLECT", InnerEnum.Collect);
    //    public static readonly HitPolicy RuleOrder = new HitPolicy("RULE_ORDER", InnerEnum.RuleOrder, "RULE ORDER");
    //    public static readonly HitPolicy OutputOrder = new HitPolicy("OUTPUT_ORDER", InnerEnum.OutputOrder, "OUTPUT ORDER");

    //    private static readonly IList<HitPolicy> ValueList = new List<HitPolicy>();

    //    static HitPolicy()
    //    {
    //        ValueList.Add(Unique);
    //        ValueList.Add(First);
    //        ValueList.Add(Priority);
    //        ValueList.Add(Any);
    //        ValueList.Add(Collect);
    //        ValueList.Add(RuleOrder);
    //        ValueList.Add(OutputOrder);
    //    }

    //    public enum InnerEnum
    //    {
    //        Unique,
    //        First,
    //        Priority,
    //        Any,
    //        Collect,
    //        RuleOrder,
    //        OutputOrder
    //    }

    //    private readonly string _nameValue;
    //    private readonly int _ordinalValue;
    //    private readonly InnerEnum _innerEnumValue;
    //    private static int _nextOrdinal = 0;


    //    private readonly string _name;

    //    internal HitPolicy(string name, InnerEnum innerEnum)
    //    {
    //        this._name = name;

    //        _nameValue = name;
    //        _ordinalValue = _nextOrdinal++;
    //        _innerEnumValue = innerEnum;
    //    }

    //    internal HitPolicy(string name, InnerEnum innerEnum, string nv)
    //    {
    //        this._name = name;

    //        _nameValue = nv;
    //        _ordinalValue = _nextOrdinal++;
    //        _innerEnumValue = innerEnum;
    //    }

    //    public override string ToString()
    //    {
    //        return _name;
    //    }


    //    public static IList<HitPolicy> Values()
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

    //    public static HitPolicy ValueOf(string name)
    //    {
    //        foreach (HitPolicy enumInstance in HitPolicy.Values())
    //        {
    //            if (enumInstance._nameValue == name)
    //            {
    //                return enumInstance;
    //            }
    //        }
    //        throw new System.ArgumentException(name);
    //    }
    //}

    public enum HitPolicy {
        [Description("UNIQUE")]
        Unique,

        [Description("FIRST")]
        First,

        [Description("PRIORITY")]
        Priority,

        [Description("ANY")]
        Any,

        [Description("COLLECT")]
        Collect,

        [Description("RULE ORDER")]
        RuleOrder,

        [Description("OUTPUT ORDER")]
        OutputOrder
    }
}