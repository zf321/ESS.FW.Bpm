using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Reflection;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.exception;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    /// </summary>
    public sealed class EnsureUtil
    {

        private static readonly EngineUtilLogger Log = ProcessEngineLogger.UtilLogger;

        public static void EnsureNotNull(string variableName, object value)
        {
            EnsureNotNull("", variableName, value);
        }

        public static void EnsureNotNull(Type exceptionClass, string variableName, object value)
        {
            EnsureNotNull(exceptionClass, null, variableName, value);
        }

        public static void EnsureNotNull(string message, string variableName, object value)
        {
            EnsureNotNull(typeof(NullValueException), message, variableName, value);
        }

        public static void EnsureNotNull(Type exceptionClass, string message, string variableName, object value) 
        {
            if (value == null)
            {
                throw GenerateException(exceptionClass, message, variableName, "is null");
            }
        }

        public static void EnsureNull(Type exceptionClass, string message, string variableName, object value) 
        {
            if (value != null)
            {
                throw GenerateException(exceptionClass, message, variableName, "is not null");
            }
        }

        //public static void EnsureNotNull(string variableName, params object[] values)
        //{
        //    EnsureNotNull("", variableName, values);
        //}

        public static void EnsureNotNull(Type exceptionClass, string variableName, params object[] values)
        {
            EnsureNotNull(exceptionClass, null, variableName, values);
        }

        //public static void EnsureNotNull(string message, string variableName, params object[] values)
        //{
        //    EnsureNotNull(typeof(NullValueException), message, variableName, values);
        //}
        
        public static void EnsureNotNull(Type exceptionClass, string message, string variableName, params object[] values)
        {
            if (values == null )
            {
                throw GenerateException(exceptionClass, message, variableName, "is null");
            }
            foreach (object value in values)
            {
                if (value == null)
                {
                    throw GenerateException(exceptionClass, message, variableName, "contains null value");
                }
            }
        }

        public static void EnsureNotEmpty(string variableName, string value)
        {
            EnsureNotEmpty("", variableName, value);
        }

        public static void EnsureNotEmpty<T>(Type exceptionClass, string variableName, string value) 
        {
            EnsureNotEmpty<T>(exceptionClass, null, variableName, value);
        }

        public static void EnsureNotEmpty(string message, string variableName, string value)
        {
            EnsureNotEmpty<ProcessEngineException>(typeof(ProcessEngineException), message, variableName, value);
        }

        public static void EnsureNotEmpty<T>(Type exceptionClass, string message, string variableName, string value) 
        {
            EnsureNotNull(exceptionClass, message, variableName, value);
            if (value.Trim().Length == 0)
            {
                throw GenerateException(exceptionClass, message, variableName, "is empty");
            }
        }
        
        public static void EnsureNotEmpty(string variableName, IEnumerable collection)
        {
            EnsureNotEmpty("", variableName, collection);
        }
        
        public static void EnsureNotEmpty(Type exceptionClass, string variableName, IEnumerable collection)
        {
            EnsureNotEmpty(exceptionClass, null, variableName, collection);
        }
        
        public static void EnsureNotEmpty(string message, string variableName, IEnumerable collection)
        {
            EnsureNotEmpty(typeof(ProcessEngineException), message, variableName, collection);
        }
        
        public static void EnsureNotEmpty(Type exceptionClass, string message, string variableName, IEnumerable collection)
        {
            EnsureNotNull(exceptionClass, message, variableName, collection);
            if (collection.Count() == 0)
            {
                throw GenerateException(exceptionClass, message, variableName, "is empty");
            }
        }
        public static void EnsureNotEmpty(string variableName, IDictionary map)
        {
            EnsureNotEmpty("", variableName, map);
        }
        
        public static void EnsureNotEmpty(Type exceptionClass, string variableName, IDictionary map)
        {
            EnsureNotEmpty(exceptionClass, null, variableName, map);
        }
        
        public static void EnsureNotEmpty(string message, string variableName, IDictionary map)
        {
            EnsureNotEmpty(typeof(ProcessEngineException), message, variableName, map);
        }
        
        public static void EnsureNotEmpty(Type exceptionClass, string message, string variableName, IDictionary map)
        {
            EnsureNotNull(exceptionClass, message, variableName, map);
            if (map.Count == 0)
            {
                throw GenerateException(exceptionClass, message, variableName, "is empty");
            }
        }

        public static void EnsureEquals(Type exceptionClass, string variableName, long obj1, long obj2)
        {
            if (obj1 != obj2)
            {
                throw GenerateException(exceptionClass, "", variableName, "value differs from expected");
            }
        }

        public static void EnsureEquals(string variableName, long obj1, long obj2)
        {
            EnsureEquals(typeof(ProcessEngineException), variableName, obj1, obj2);
        }

        public static void EnsurePositive(string variableName, long? value)
        {
            EnsurePositive("", variableName, value);
        }

        public static void EnsurePositive(Type exceptionClass, string variableName, long? value)
        {
            EnsurePositive(exceptionClass, null, variableName, value);
        }

        public static void EnsurePositive(string message, string variableName, long? value)
        {
            EnsurePositive(typeof(ProcessEngineException), message, variableName, value);
        }

        public static void EnsurePositive(Type exceptionClass, string message, string variableName, long? value)
        {
            EnsureNotNull(exceptionClass, variableName, value);
            if (value <= 0)
            {
                throw GenerateException(exceptionClass, message, variableName, "is not greater than 0");
            }
        }

        public static void EnsureGreaterThanOrEqual(string variableName, long value1, long value2)
        {
            EnsureGreaterThanOrEqual("", variableName, value1, value2);
        }

        public static void EnsureGreaterThanOrEqual(string message, string variableName, long value1, long value2)
        {
            EnsureGreaterThanOrEqual(typeof(ProcessEngineException), message, variableName, value1, value2);
        }

        public static void EnsureGreaterThanOrEqual(Type exceptionClass, string message, string variableName, long value1, long value2)
        {
            if (value1 < value2)
            {
                throw GenerateException(exceptionClass, message, variableName, "is not greater than or equal to " + value2);
            }
        }

        public static void EnsureInstanceOf(string variableName, object value, Type expectedClass)
        {
            EnsureInstanceOf("", variableName, value, expectedClass);
        }

        public static void EnsureInstanceOf(Type exceptionClass, string variableName, object value, Type expectedClass)
        {
            EnsureInstanceOf(exceptionClass, null, variableName, value, expectedClass);
        }

        public static void EnsureInstanceOf(string message, string variableName, object value, Type expectedClass)
        {
            EnsureInstanceOf(typeof(ProcessEngineException), message, variableName, value, expectedClass);
        }

        public static void EnsureInstanceOf(Type exceptionClass, string message, string variableName, object value, Type expectedClass)
        {
            EnsureNotNull(exceptionClass, message, variableName, value);
            Type valueClass = value.GetType();
            if (!expectedClass.IsAssignableFrom(valueClass))
            {
                throw GenerateException(exceptionClass, message, variableName, "has class " + valueClass.FullName + " and not " + expectedClass.FullName);
            }
        }

        public static void EnsureOnlyOneNotNull(string message, params object[] values)
        {
            EnsureOnlyOneNotNull(typeof(NullValueException), message, values);
        }

        public static void EnsureOnlyOneNotNull(Type exceptionClass, string message, params object[] values)
        {
            bool oneNotNull = false;
            foreach (object value in values)
            {
                if (value != null)
                {
                    if (oneNotNull)
                    {
                        throw GenerateException(exceptionClass, null, null, message);
                    }
                    oneNotNull = true;
                }
            }
            if (!oneNotNull)
            {
                throw GenerateException(exceptionClass, null, null, message);
            }
        }

        public static void EnsureAtLeastOneNotNull(string message, params object[] values)
        {
            EnsureAtLeastOneNotNull(typeof(NullValueException), message, values);
        }

        public static void EnsureAtLeastOneNotNull(Type exceptionClass, string message, params object[] values)
        {
            foreach (object value in values)
            {
                if (value != null)
                {
                    return;
                }
            }
            throw GenerateException(exceptionClass, null, null, message);
        }

        public static void EnsureAtLeastOneNotEmpty(string message, params string[] values)
        {
            EnsureAtLeastOneNotEmpty(typeof(ProcessEngineException), message, values);
        }

        public static void EnsureAtLeastOneNotEmpty(Type exceptionClass, string message, params string[] values)
        {
            foreach (string value in values)
            {
                if (!string.ReferenceEquals(value, null) && value.Length > 0)
                {
                    return;
                }
            }
            throw GenerateException(exceptionClass, null, null, message);
        }

        public static void EnsureNotContainsEmptyString(string variableName, IEnumerable<string> values)
        {
            EnsureNotContainsEmptyString((string)null, variableName, values);
        }

        public static void EnsureNotContainsEmptyString(string message, string variableName, IEnumerable<string> values)
        {
            EnsureNotContainsEmptyString(typeof(NotValidException), message, variableName, values);
        }

        public static void EnsureNotContainsEmptyString(Type exceptionClass, string variableName, IEnumerable<string> values)
        {
            EnsureNotContainsEmptyString(exceptionClass, null, variableName, values);
        }

        public static void EnsureNotContainsEmptyString(Type exceptionClass, string message, string variableName, IEnumerable<string> values)
        {
            EnsureNotNull(exceptionClass, message, variableName, values);
            foreach (string value in values)
            {
                if (value.Length == 0)
                {
                    throw GenerateException(exceptionClass, message, variableName, "contains empty string");
                }
            }
        }

        public static void EnsureNotContainsNull<T1>(string variableName, IEnumerable<T1> values)
        {
            EnsureNotContainsNull((string)null, variableName, values);
        }

        public static void EnsureNotContainsNull<T1>(string message, string variableName, IEnumerable<T1> values)
        {
            EnsureNotContainsNull(typeof(NullValueException), message, variableName, values);
        }

        public static void EnsureNotContainsNull<T1>(Type exceptionClass, string variableName, IEnumerable<T1> values)
        {
            EnsureNotContainsNull(exceptionClass, null, variableName, values);
        }

        public static void EnsureNotContainsNull<T1>(Type exceptionClass, string message, string variableName, IEnumerable<T1> values)
        {
            EnsureNotNull(exceptionClass, message, variableName, (new object[values.Count()]));
        }
        
        public static void EnsureNumberOfElements(string variableName, IEnumerable collection, int elements)
        {
            EnsureNumberOfElements("", variableName, collection, elements);
        }
        
        public static void EnsureNumberOfElements(string message, string variableName, IEnumerable collection, int elements)
        {
            EnsureNumberOfElements(typeof(ProcessEngineException), message, variableName, collection, elements);
        }
        
        public static void EnsureNumberOfElements(Type exceptionClass, string variableName, IEnumerable collection, int elements)
        {
            EnsureNumberOfElements(exceptionClass, "", variableName, collection, elements);
        }
        
        public static void EnsureNumberOfElements(Type exceptionClass, string message, string variableName, IEnumerable collection, int elements)
        {
            EnsureNotNull(exceptionClass, message, variableName, collection);
            if (collection.Count() != elements)
            {
                throw GenerateException(exceptionClass, message, variableName, "does not have " + elements + " elements");
            }
        }

        public static void EnsureValidIndividualResourceId(string message, string id)
        {
            EnsureValidIndividualResourceId(typeof(ProcessEngineException), message, id);
        }

        public static void EnsureValidIndividualResourceId(Type exceptionClass, string message, string id)
        {
            EnsureNotNull(exceptionClass, message, "id", id);
            if (AuthorizationFields.Any.Equals(id))
            {
                throw GenerateException(exceptionClass, message, "id", "cannot be " + AuthorizationFields.Any + ". " + AuthorizationFields.Any + " is a reserved identifier.");
            }
        }

        public static void EnsureValidIndividualResourceIds(string message, IEnumerable<string> ids)
        {
            EnsureValidIndividualResourceIds(typeof(ProcessEngineException), message, ids);
        }

        public static void EnsureValidIndividualResourceIds(Type exceptionClass, string message, IEnumerable<string> ids)
        {
            EnsureNotNull(exceptionClass, message, "id", ids);
            foreach (string id in ids)
            {
                EnsureValidIndividualResourceId(exceptionClass, message, id);
            }
        }

        private static ProcessEngineException GenerateException(Type exceptionClass, string message, string variableName, string description)
        {
            string formattedMessage = FormatMessage(message, variableName, description);

            try
            {
                ConstructorInfo constructor = exceptionClass.GetConstructor(new Type[]{typeof(string)});

                return  (ProcessEngineException) constructor.Invoke(new object[]{formattedMessage});

            }
            catch (System.Exception e)
            {
                throw Log.ExceptionWhileInstantiatingClass(exceptionClass.FullName, e);
            }

        }

        protected internal static string FormatMessage(string message, string variableName, string description)
        {
            return FormatMessageElement(message, ": ") + FormatMessageElement(variableName, " ") + description;
        }

        protected internal static string FormatMessageElement(string element, string delimiter)
        {
            if (!string.ReferenceEquals(element, null) && element.Length > 0)
            {
                return element + delimiter;
            }
            else
            {
                return "";
            }
        }

        public static void EnsureActiveCommandContext(string operation)
        {
            if (Context.CommandContext == null)
            {
                throw Log.NotInsideCommandContext(operation);
            }
        }

    }
}
