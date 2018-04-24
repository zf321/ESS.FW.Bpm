


namespace ESS.FW.Bpm.Engine.Impl.Variable
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.Variable.type.ITypedValue.BOOLEAN;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.Variable.type.ITypedValue.BYTES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.Variable.type.ITypedValue.DATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.Variable.type.ITypedValue.DOUBLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.Variable.type.ITypedValue.FILE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.Variable.type.ITypedValue.INTEGER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.Variable.type.ITypedValue.LONG;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.Variable.type.ITypedValue.NULL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.Variable.type.ITypedValue.NUMBER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.Variable.type.ITypedValue.OBJECT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.Variable.type.ITypedValue.SHORT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.Variable.type.ITypedValue.STRING;


/// <summary>
///     Resolves ITypedValue by name.
///     
/// </summary>
//public class ITypeValueResolverImpl : ITypeValueResolver
//{
//    protected internal IDictionary<string, IITypeValue> knownTypes = new Dictionary<string, IITypeValue>();

//    public ITypeValueResolverImpl()
//    {
//        addType(ITypedValue.BOOLEAN);
//        addType(ITypedValue.BYTES);
//        addType(ITypedValue.DATE);
//        addType(ITypedValue.DOUBLE);
//        addType(ITypedValue.INTEGER);
//        addType(ITypedValue.LONG);
//        addType(ITypedValue.NULL);
//        addType(ITypedValue.SHORT);
//        addType(ITypedValue.STRING);
//        addType(ITypedValue.OBJECT);
//        addType(ITypedValue.NUMBER);
//        addType(ITypedValue.FILE);
//    }

//    public virtual void addType(IITypeValue type)
//    {
//        knownTypes[type.Name] = type;
//    }

//    public virtual IITypeValue typeForName(string typeName)
//    {
//        return knownTypes[typeName];
//    }

//    public virtual ICollection<IITypeValue> getSubTypes(IITypeValue type)
//    {
//        IList<IITypeValue> types = new List<IITypeValue>();

//        ISet<IITypeValue> validParents = new HashSet<IITypeValue>();
//        validParents.Add(type);

//        foreach (IITypeValue knownType in knownTypes.Values)
//        {
//            if (validParents.Contains(knownType.Parent))
//            {
//                validParents.Add(knownType);

//                if (!knownType.Abstract)
//                {
//                    types.Add(knownType);
//                }
//            }
//        }

//        return types;
//    }
//}
}