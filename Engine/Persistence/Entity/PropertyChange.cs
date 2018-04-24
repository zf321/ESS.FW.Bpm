using System;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    ///     Contains data about a property change.
    /// </summary>
    public class PropertyChange
    {
        /// <summary>
        ///     the empty change
        /// </summary>
        public static readonly PropertyChange EmptyChange = new PropertyChange(null, null, null);

        public PropertyChange(string propertyName, object orgValue, object newValue)
        {
            this.PropertyName = propertyName;
            this.OrgValue = orgValue;
            this.NewValue = newValue;
        }

        public virtual string PropertyName { get; set; }


        public virtual object OrgValue { get; set; }


        public virtual object NewValue { get; set; }


        public virtual string NewValueString
        {
            get { return ValueAsString(NewValue); }
        }

        public virtual string OrgValueString
        {
            get { return ValueAsString(OrgValue); }
        }

        protected internal virtual string ValueAsString(object value)
        {
            if (value == null)
                return null;
            if (value is DateTime)
                return Convert.ToString(((DateTime) value).Ticks);
            return value.ToString();
        }
    }
}