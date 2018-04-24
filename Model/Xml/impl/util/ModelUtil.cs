using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type;
using ESS.FW.Bpm.Model.Xml.impl.type.attribute;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;


namespace ESS.FW.Bpm.Model.Xml.impl.util
{

    /// <summary>
    /// Some Helpers useful when handling model elements.
    /// </summary>
    public sealed class ModelUtil
    {
        //TODO 入口 IModelElementInstance解析
        /// <summary>
        /// Returns the <seealso cref="ModelElementInstanceImpl ModelElement"/> for a DOM element.
        /// If the model element does not yet exist, it is created and linked to the DOM.
        /// </summary>
        /// <param name="domElement"> the child element to create a new <seealso cref="ModelElementInstanceImpl ModelElement"/> for </param>
        /// <returns> the child model element </returns>
        public static IModelElementInstance GetModelElement(IDomElement domElement, IModelInstance modelInstance)
        {
            IModelElementInstance modelElement = domElement.ModelElementInstance;
            if (modelElement == null)
            {
                IModelElementType modelType = GetModelElement(domElement, modelInstance, domElement.NamespaceUri);
                modelElement = ((ModelElementTypeImpl)modelType).NewInstance(modelInstance, domElement);
                domElement.ModelElementInstance = modelElement;
            }
            return modelElement;
        }

        protected internal static IModelElementType GetModelElement(IDomElement domElement, IModelInstance modelInstance, string namespaceUri)
        {
            string localName = domElement.LocalName;
            IModelElementType modelType = modelInstance.Model.GetTypeForName(namespaceUri, localName);

            if (modelType == null)
            {

                IModel model = modelInstance.Model;
                string actualNamespaceUri = model.GetActualNamespace(namespaceUri);

                if (!string.ReferenceEquals(actualNamespaceUri, null))
                {
                    modelType = GetModelElement(domElement, modelInstance, actualNamespaceUri);
                }
                else
                {
                    modelType = ((ModelInstanceImpl)modelInstance).RegisterGenericType(namespaceUri, localName);
                }

            }
            return modelType;
        }

        public static QName GetQName(string namespaceUri, string localName)
        {
            return new QName(namespaceUri, localName);
        }

        public static void EnsureInstanceOf(object instance, Type type)
        {
            if (!type.IsAssignableFrom(instance.GetType()))
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                throw new ModelException("Object is not instance of type " + type.FullName);
            }
        }

        // String to primitive type converters ////////////////////////////////////

        public static bool ValueAsBoolean(string rawValue)
        {
            return bool.Parse(rawValue);
        }

        public static int ValueAsInteger(string rawValue)
        {
            try
            {
                return int.Parse(rawValue);
            }
            catch (System.FormatException)
            {
                throw new ModelTypeException(rawValue, typeof(int));
            }
        }

        public static float ValueAsFloat(string rawValue)
        {
            try
            {
                return float.Parse(rawValue);
            }
            catch (System.FormatException)
            {
                throw new ModelTypeException(rawValue, typeof(float));
            }
        }

        public static double ValueAsDouble(string rawValue)
        {
            try
            {
                return double.Parse(rawValue);
            }
            catch (System.FormatException)
            {
                throw new ModelTypeException(rawValue, typeof(Double));
            }
        }

        public static short ValueAsShort(string rawValue)
        {
            try
            {
                return short.Parse(rawValue);
            }
            catch (System.FormatException)
            {
                throw new ModelTypeException(rawValue, typeof(short));
            }
        }

        // primitive type to string converters //////////////////////////////////////

        public static string ValueAsString(bool booleanValue)
        {
            return Convert.ToString(booleanValue);
        }

        public static string ValueAsString(int integerValue)
        {
            return Convert.ToString(integerValue);
        }

        public static string ValueAsString(float floatValue)
        {
            return Convert.ToString(floatValue);
        }

        public static string ValueAsString(double doubleValue)
        {
            return Convert.ToString(doubleValue);
        }

        public static string ValueAsString(short shortValue)
        {
            return Convert.ToString(shortValue);
        }

        /// <summary>
        /// 从view中获取所有ModelElement实例
        /// Get a collection of all model element instances in a view
        /// </summary>
        /// <param name="view"> the collection of DOM elements to find the model element instances for </param>
        /// <param name="model"> the model of the elements </param>
        /// <returns> the collection of model element instances of the view </returns>
        public static IList<T> GetModelElementCollection<T>(IList<IDomElement> view, IModelInstance model) where T : IModelElementInstance
        {
            IList<T> resultList = new List<T>();
            foreach (IDomElement element in view)
            {
                //var t = getModelElement(element, model);
                //TODO 4.20 类型转换异常
                resultList.Add((T)GetModelElement(element, model));
            }
            return resultList;
        }

        /// <summary>
        /// Find the index of the type of a model element in a list of element types
        /// </summary>
        /// <param name="modelElement"> the model element which type is searched for </param>
        /// <param name="childElementTypes"> the list to search the type </param>
        /// <returns> the index of the model element type in the list or -1 if it is not found </returns>
        public static int GetIndexOfElementType(IModelElementInstance modelElement, IList<IModelElementType> childElementTypes)
        {
            for (int index = 0; index < childElementTypes.Count; index++)
            {
                IModelElementType childElementType = childElementTypes[index];
                Type instanceType = childElementType.InstanceType;
                if (instanceType.IsAssignableFrom(modelElement.GetType()))
                {
                    return index;
                }
            }
            ICollection<string> childElementTypeNames = new List<string>();
            foreach (IModelElementType childElementType in childElementTypes)
            {
                childElementTypeNames.Add(childElementType.TypeName);
            }
            throw new ModelException("New child is not a valid child element type: " + modelElement.ElementType.TypeName + "; valid types are: " + childElementTypeNames);
        }

        /// <summary>
        /// Calculate a collection of all extending types for the given base types
        /// </summary>
        /// <param name="baseTypes"> the collection of types to calculate the union of all extending types </param>
        public static ICollection<IModelElementType> CalculateAllExtendingTypes(IModel model, ICollection<IModelElementType> baseTypes)
        {
            ISet<IModelElementType> allExtendingTypes = new HashSet<IModelElementType>();
            foreach (IModelElementType baseType in baseTypes)
            {
                ModelElementTypeImpl modelElementTypeImpl = (ModelElementTypeImpl)model.GetType(baseType.InstanceType);
                modelElementTypeImpl.ResolveExtendingTypes(allExtendingTypes);
            }
            return allExtendingTypes;
        }

        /// <summary>
        /// Calculate a collection of all base types for the given type
        /// </summary>
        public static ICollection<IModelElementType> CalculateAllBaseTypes(IModelElementType type)
        {
            IList<IModelElementType> baseTypes = new List<IModelElementType>();
            ModelElementTypeImpl typeImpl = (ModelElementTypeImpl)type;
            typeImpl.ResolveBaseTypes(baseTypes);
            return baseTypes;
        }
        //TODO 可能又类型判断错误 设置ModelElementId id.DefaultValue is string
        /// <summary>
        /// Set unique identifier if the type has a String id attribute
        /// </summary>
        /// <param name="type"> the type of the model element </param>
        /// <param name="modelElementInstance"> the model element instance to set the id </param>
        public static void SetGeneratedUniqueIdentifier(IModelElementType type, IModelElementInstance modelElementInstance)
        {
            //AttributeBox id = type.GetAttribute("id");
            IAttribute/*<String>*/ id = type.GetAttribute("id");
            //if (id != null && id.ValueType ==typeof(string) && id.IdAttribute)
            if (id != null && id.IdAttribute)
            {
                ((StringAttribute)id).SetValue(modelElementInstance, ModelUtil.GetUniqueIdentifier(type));
            }
        }

        public static string GetUniqueIdentifier(IModelElementType type)
        {
            return type.TypeName + "_" + Guid.NewGuid();
        }
    }
}