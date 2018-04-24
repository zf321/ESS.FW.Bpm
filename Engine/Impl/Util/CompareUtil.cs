using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///     Util class for comparisons.
    ///      Filip Hrisafov
    /// </summary>
    public class CompareUtil
    {
        /// <summary>
        ///     Checks if any of the values are not in an ascending order. The check is done based on the
        ///     <seealso cref="Comparable#compareTo(Object)" /> method.
        ///     E.g. if we have {@code minPriority = 10}, {@code priority = 13} and {@code maxPriority = 5} and
        ///     {@code Integer[] values = {minPriority, priority, maxPriority}}. Then a call to
        ///     <seealso cref="CompareUtil#areNotInAscendingOrder(Comparable[] values)" />
        ///     will return {@code true}
        /// </summary>
        /// <param name="values"> to validate </param>
        /// @param
        /// <T>
        ///     the type of the comparable </param>
        ///     <returns>
        ///         {@code false} if the not null values are in an ascending order or all the values are null, {@code true}
        ///         otherwise
        ///     </returns>
        public static bool AreNotInAscendingOrder<T>(params T[] values) where T : IComparable<T>
        {
            var excluding = false;
            if (values != null)
                excluding = AreNotInAscendingOrder<T>(new List<T>(values));
            return excluding;
        }

        /// <summary>
        ///     Checks if any of the values are not in an ascending order. The check is done based on the
        ///     <seealso cref="Comparable#compareTo(Object)" /> method.
        ///     E.g. if we have {@code minPriority = 10}, {@code priority = 13} and {@code maxPriority = 5} and
        ///     {@code List
        ///     <Integer>
        ///         values = {minPriority, priority, maxPriority}}. Then a call to
        ///         <seealso cref="CompareUtil#areNotInAscendingOrder(List values)" />
        ///         will return {@code true}
        /// </summary>
        /// <param name="values"> to validate </param>
        /// @param
        /// <T>
        ///     the type of the comparable </param>
        ///     <returns>
        ///         {@code false} if the not null values are in an ascending order or all the values are null, {@code true}
        ///         otherwise
        ///     </returns>
        public static bool AreNotInAscendingOrder<T>(IList<T> values) where T : IComparable<T>
        {
            var lastNotNull = -1;
            for (var i = 0; i < values.Count; i++)
            {
                var value = values[i];

                if (value != null)
                {
                    if ((lastNotNull != -1) && (values[lastNotNull].CompareTo(value) > 0))
                        return true;

                    lastNotNull = i;
                }
            }

            return false;
        }

        /// <summary>
        ///     Checks if the element is not contained within the list of values. If the element, or the list are null then true is
        ///     returned.
        /// </summary>
        /// <param name="element"> to check </param>
        /// <param name="values"> to check in </param>
        /// @param
        /// <T>
        ///     the type of the element </param>
        ///     <returns>
        ///         {@code true} if the element and values are not {@code null} and the values does not contain the element,
        ///         {@code false} otherwise
        ///     </returns>
        public static bool ElementIsNotContainedInList<T>(T element, ICollection<T> values)
        {
            if ((element != null) && (values != null))
                return !values.Contains(element);
            return false;
        }

        /// <summary>
        ///     Checks if the element is contained within the list of values. If the element, or the list are null then true is
        ///     returned.
        /// </summary>
        /// <param name="element"> to check </param>
        /// <param name="values"> to check in </param>
        /// @param
        /// <T>
        ///     the type of the element </param>
        ///     <returns>
        ///         {@code true} if the element and values are not {@code null} and the values does not contain the element,
        ///         {@code false} otherwise
        ///     </returns>
        public static bool ElementIsNotContainedInArray<T>(T element, params T[] values)
        {
            if ((element != null) && (values != null))
                return ElementIsNotContainedInList(element, values);
            return false;
        }

        /// <summary>
        ///     Checks if the element is contained within the list of values.
        /// </summary>
        /// <param name="element"> to check </param>
        /// <param name="values"> to check in </param>
        /// @param
        /// <T>
        ///     the type of the element </param>
        ///     <returns>
        ///         {@code true} if the element and values are not {@code null} and the values contain the element,
        ///         {@code false} otherwise
        ///     </returns>
        public static bool ElementIsContainedInList<T>(T element, ICollection<T> values)
        {
            if ((element != null) && (values != null))
                return values.Contains(element);
            return false;
        }

        /// <summary>
        ///     Checks if the element is contained within the list of values.
        /// </summary>
        /// <param name="element"> to check </param>
        /// <param name="values"> to check in </param>
        /// @param
        /// <T>
        ///     the type of the element </param>
        ///     <returns>
        ///         {@code true} if the element and values are not {@code null} and the values contain the element,
        ///         {@code false} otherwise
        ///     </returns>
        public static bool ElementIsContainedInArray<T>(T element, params T[] values)
        {
            if ((element != null) && (values != null))
                return ElementIsContainedInList(element, values);
            return false;
        }

        /// <summary>
        ///     Returns any element if obj1.compareTo(obj2) == 0
        /// </summary>
        public static T Min<T>(T obj1, T obj2) where T : IComparable<T>
        {
            return obj1.CompareTo(obj2) <= 0 ? obj1 : obj2;
        }

        /// <summary>
        ///     Returns any element if obj1.compareTo(obj2) == 0
        /// </summary>
        public static T Max<T>(T obj1, T obj2) where T : IComparable<T>
        {
            return obj1.CompareTo(obj2) >= 0 ? obj1 : obj2;
        }
    }
}