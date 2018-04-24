using System.Collections.Generic;
using System.ComponentModel;

namespace ESS.FW.Bpm.Model.Dmn
{

    //public sealed class DecisionTableOrientation
    //{

    //    public static readonly DecisionTableOrientation RuleAsRow = new DecisionTableOrientation("Rule_as_Row", InnerEnum.RuleAsRow, "Rule-as-Row");
    //    public static readonly DecisionTableOrientation RuleAsColumn = new DecisionTableOrientation("Rule_as_Column", InnerEnum.RuleAsColumn, "Rule-as-Column");
    //    public static readonly DecisionTableOrientation CrossTable = new DecisionTableOrientation("CrossTable", InnerEnum.CrossTable);

    //    private static readonly IList<DecisionTableOrientation> ValueList = new List<DecisionTableOrientation>();

    //    static DecisionTableOrientation()
    //    {
    //        ValueList.Add(RuleAsRow);
    //        ValueList.Add(RuleAsColumn);
    //        ValueList.Add(CrossTable);
    //    }

    //    public enum InnerEnum
    //    {
    //        RuleAsRow,
    //        RuleAsColumn,
    //        CrossTable
    //    }

    //    private readonly string _nameValue;
    //    private readonly int _ordinalValue;
    //    private readonly InnerEnum _innerEnumValue;
    //    private static int _nextOrdinal = 0;

    //    private readonly string _name;

    //    internal DecisionTableOrientation(string name, InnerEnum innerEnum)
    //    {
    //        _name = name;

    //        _nameValue = name;
    //        _ordinalValue = _nextOrdinal++;
    //        _innerEnumValue = innerEnum;
    //    }

    //    internal DecisionTableOrientation(string name, InnerEnum innerEnum, string nameValue)
    //    {
    //        this._name = name;
    //        _nameValue = nameValue;
    //        _ordinalValue = _nextOrdinal++;
    //        _innerEnumValue = innerEnum;
    //    }

    //    public override string ToString()
    //    {
    //        return _name;
    //    }

    //    public static IList<DecisionTableOrientation> Values()
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

    //    public static DecisionTableOrientation ValueOf(string name)
    //    {
    //        foreach (DecisionTableOrientation enumInstance in DecisionTableOrientation.Values())
    //        {
    //            if (enumInstance._nameValue == name)
    //            {
    //                return enumInstance;
    //            }
    //        }
    //        throw new System.ArgumentException(name);
    //    }
    //}

    public enum DecisionTableOrientation {

        [Description("Rule-as-Row")]
        RuleAsRow,

        [Description("Rule-as-Column")]
        RuleAsColumn,

        [Description("CrossTable")]
        CrossTable

    }
}