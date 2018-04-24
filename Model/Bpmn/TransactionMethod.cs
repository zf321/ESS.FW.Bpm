using System.Collections.Generic;
using System.ComponentModel;

namespace ESS.FW.Bpm.Model.Bpmn
{           
    //public sealed class TransactionMethod
    //{

    //    public static readonly TransactionMethod Compensate = new TransactionMethod("Compensate", InnerEnum.Compensate, "##Compensate");
    //    public static readonly TransactionMethod Image = new TransactionMethod("Image", InnerEnum.Image, "##Image");
    //    public static readonly TransactionMethod Store = new TransactionMethod("Store", InnerEnum.Store, "##Store");

    //    private static readonly IList<TransactionMethod> ValueList = new List<TransactionMethod>();

    //    static TransactionMethod()
    //    {
    //        ValueList.Add(Compensate);
    //        ValueList.Add(Image);
    //        ValueList.Add(Store);
    //    }

    //    public enum InnerEnum
    //    {
    //        Compensate,
    //        Image,
    //        Store
    //    }

    //    private readonly string _nameValue;
    //    private readonly int _ordinalValue;
    //    private readonly InnerEnum _innerEnumValue;
    //    private static int _nextOrdinal = 0;

    //    private readonly string _name;

    //    internal TransactionMethod(string name, InnerEnum innerEnum)
    //    {
    //        this._name = name;

    //        _nameValue = name;
    //        _ordinalValue = _nextOrdinal++;
    //        _innerEnumValue = innerEnum;
    //    }

    //    internal TransactionMethod(string name, InnerEnum innerEnum, string namevalue)
    //    {
    //        this._name = name;

    //        _nameValue = namevalue;
    //        _ordinalValue = _nextOrdinal++;
    //        _innerEnumValue = innerEnum;
    //    }

    //    public override string ToString()
    //    {
    //        return _name;
    //    }


    //    public static IList<TransactionMethod> Values()
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

    //    public static TransactionMethod ValueOf(string name)
    //    {
    //        foreach (TransactionMethod enumInstance in TransactionMethod.Values())
    //        {
    //            if (enumInstance._nameValue == name)
    //            {
    //                return enumInstance;
    //            }
    //        }
    //        throw new System.ArgumentException(name);
    //    }
    //}

    public enum TransactionMethod {

        [Description("##Compensate")]
        Compensate,

        [Description("##Image")]
        Image,

        [Description("##Store")]
        Store
    }
}